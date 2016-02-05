using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using System.Xml.Serialization;

namespace TE.Apps.Staging
{
	public class StagingService : ServiceBase
	{
		#region Public Constants
		/// <summary>
		/// The name of the service.
		/// </summary>
		public const string MyServiceName = "Backup Staging Manager";
		#endregion

		#region Private Constants
		/// <summary>
		/// The application data directory name.
		/// </summary>
		private const string AppDataDirectoryName = "Backup Staging";
		/// <summary>
		/// The name of the settings file.
		/// </summary>
		private const string SettingsFileName = "settings.xml";		
		#endregion
		
		#region Private Variables
		/// <summary>
		/// The settings used for the service.
		/// </summary>
		private SettingsFile settingsFile;		
		/// <summary>
		/// The FileSystemWatcher that monitors the source directory.
		/// </summary>
		private FileSystemWatcher watcher;
		/// <summary>
		/// The timer used to cleanup the source directory.
		/// </summary>
		private System.Timers.Timer timer;
		/// <summary>
		/// The queue that will contain the files to be moved.
		/// </summary>
		private Queue stagingFilesQueue;
		/// <summary>
		/// The background worker that performs the file moves.
		/// </summary>
		private BackgroundWorker bg;
		#endregion
		
		#region Properties
//		/// <summary>
//		/// Gets or sets the source staging directory.
//		/// </summary>
//		public string SourceFolder { get; set; }
//		
//		/// <summary>
//		/// Gets or sets the destination directory.
//		/// </summary>
//		public string DestinationFolder { get; set; }
		
		public List<Location> FileLocations { get; set; }
		/// <summary>
		/// Gets or sets the copy retry count.
		/// </summary>
		public int RetryMoveCount { get; set;}
		
		/// <summary>
		/// Gets or sets the copy retry wait in seconds.
		/// </summary>
		public int RetryMoveWait {get; set; }
		#endregion
		
		#region Constructors
		public StagingService()
		{
			InitializeComponent();
		}
		#endregion
		
		#region Private Functions
		/// <summary>
		/// Cleans up objects when the service is stopped.
		/// </summary>
		private void Deinitialize()
		{			
			this.bg.CancelAsync();
			this.bg.Dispose();
			
			this.timer.Enabled = false;
			this.timer.Close();
			this.timer.Dispose();
			
			this.watcher.EnableRaisingEvents = false;
			this.watcher.Dispose();
			
			this.bg = null;
			this.timer = null;
			this.watcher = null;
		}
		
		/// <summary>
		/// Deserializes the settings from the settings file.
		/// </summary>
		/// <param name="settingsFilePath">
		/// [in] Full path to the settings file.
		/// </param>
		/// <returns>
		/// A <see cref="TE.Apps.Staging.SettingsFile"/> object.
		/// </returns>
		private SettingsFile DeserializeSettings(string settingsFilePath)
		{
			XmlSerializer xs = new XmlSerializer(typeof(SettingsFile));		
			
			using (Stream s = File.OpenRead(settingsFilePath))
			{
				return (SettingsFile)xs.Deserialize (s);
			}
		}
		
		/// <summary>
		/// Gets the settings from the settings file.
		/// </summary>
		/// <returns>
		/// A <see cref="TE.Apps.Staging.SettingsFile"/> object.
		/// </returns>
		private SettingsFile GetSettings()
		{
			// Get the settings file path and check if it exists
			string settingsFilePath = GetSettingsFilePath();
			if (!File.Exists(settingsFilePath))
			{
				// Create some default values if the settings file doesn't exist
				SettingsFile settings = new SettingsFile();
				
				Location location =	new Location("C:\\", "C:\\");				
				List<Location> locations = new List<Location>();
				locations.Add(location);
				
				settings.Locations = locations;
				//settings.SourceDirectory = string.Empty;
				//settings.DestinationDirectory = string.Empty;
				settings.MoveRetryCount = 5;
				settings.MoveRetryWait = 30;
				
				// Save the settings to the file
				this.SerializeSettings(settingsFilePath, settings);
			}
			
			// Read the settings from the file
			return this.DeserializeSettings(settingsFilePath);
		}
		
		/// <summary>
		/// Initializes objects when the service is started.
		/// </summary>
		private void Initialize()
		{
			this.settingsFile = new SettingsFile();
			this.settingsFile = this.GetSettings();
			
			// Initialize the properties
//			this.SourceFolder = this.settingsFile.SourceDirectory;
//			this.DestinationFolder = this.settingsFile.DestinationDirectory;
			
			this.FileLocations = this.settingsFile.Locations;
			this.RetryMoveCount = this.settingsFile.MoveRetryCount;
			this.RetryMoveWait = this.settingsFile.MoveRetryWait;
			
			// Initialize the file move queue
			this.stagingFilesQueue = new Queue();
			
			// Initialize the BackgroundWorker object that will perform the
			// moving of the staging files
			this.bg = new BackgroundWorker();			
			this.bg.WorkerSupportsCancellation = true;
			this.bg.DoWork += new DoWorkEventHandler(MoveStagingFiles);
			this.bg.RunWorkerAsync();

			// Initialize the timer that will cleanup any empty directories
			// in the staging directory
			this.timer = new System.Timers.Timer();
			this.timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			this.timer.Interval = 10000;
			this.timer.Enabled = true;			
			
			// Initialize the FileSystemWatcher object to watch the staging
			// directory for changes
			this.watcher = new FileSystemWatcher();
			this.watcher.Path = ((Location)this.FileLocations[0]).Source;
			this.watcher.IncludeSubdirectories = true;
			this.watcher.NotifyFilter = 
				NotifyFilters.LastAccess | 
				NotifyFilters.LastWrite | 
				NotifyFilters.FileName | 
				NotifyFilters.DirectoryName;
			this.watcher.Filter = "*.*";
			this.watcher.Changed += new FileSystemEventHandler(this.OnChanged);
			this.watcher.Created += new FileSystemEventHandler(this.OnChanged);
			this.watcher.EnableRaisingEvents = true;		
		}

		/// <summary>
		/// Initializes objects when the service created.
		/// </summary>
		private void InitializeComponent()
		{
			this.ServiceName = MyServiceName;
		}

		/// <summary>
		/// Process the directories and deletes any directories that contain no
		/// files and haven't been written to in 10 minutes.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when the start location path is null or empty.
		/// </exception>
		private static void ProcessDirectory(string startLocation)
		{
			
			if (string.IsNullOrEmpty(startLocation))
			{
				throw new ArgumentNullException(startLocation);
			}
			
			foreach (var directory in Directory.GetDirectories(startLocation))
			{
				ProcessDirectory(directory);
				
				// Find empty directories
				if (!Directory.EnumerateFileSystemEntries(directory).Any())
				{
					DateTime dt = Directory.GetLastWriteTime(directory);
					Console.WriteLine("The last write time for this directory {1} was {0}", 
						directory,
						dt);
					
					// Delete folders that were last written to more than 2 minutes ago
					if (dt < DateTime.Now.AddMinutes(-10))
					{
						Directory.Delete(directory, false);
					}
				}
			}
		}	
		
		/// <summary>
		/// Moves the staging files from the source directory to the destination
		/// directory.
		/// </summary>
		private void MoveStagingFiles(object sender, DoWorkEventArgs e)
		{
			if (this.bg == null || this.stagingFilesQueue == null)
			{
				return;
			}
			
			while (!this.bg.CancellationPending)
			{
				if (this.stagingFilesQueue.Count > 0)
				{				
					while (this.stagingFilesQueue.Count != 0)
					{	
						// Get the next file in the queue
						StagingFile stagingFile = 
							(StagingFile)this.stagingFilesQueue.Dequeue();
						
						int i = 0;					
						while (i < this.RetryMoveCount)
						{						
							try
							{
								stagingFile.Move();
								i = this.RetryMoveCount;
							}
							catch (IOException)
							{
								// Wait for a specified number of milliseconds
								// and then increment the retry counter
								Thread.Sleep(this.RetryMoveWait * 1000);
								i++;
								
								// If the retry copy count reaches the limit,
								// then re-enqueue the file to try again later
								if (i == this.RetryMoveCount)
								{
									this.stagingFilesQueue.Enqueue(stagingFile);
								}								
							}
						}
					}
				}
				else
				{
					System.Threading.Thread.Sleep(100);
				}
			}
			
			e.Cancel = true;
		}	
		
		/// <summary>
		/// Saves the settings to the settings file.
		/// </summary>
		/// <param name="settings">
		/// [in] The settings to save to the file.
		/// </param>
		private void SaveSettings(SettingsFile settings)
		{
			string settingsFilePath = GetSettingsFilePath();
			
			if (string.IsNullOrEmpty(settingsFilePath))
			{
				throw new Exception(
					"The settings file path could not be determined.");
			}
			
			this.SerializeSettings(settingsFilePath, settings);

		}
		
		/// <summary>
		/// Serializes the settings into the settings file.
		/// </summary>
		/// <param name="settingsFilePath">
		/// [in] Full path to the settings file.
		/// </param>
		/// <param name="settings">
		/// [in] The settings to serialize into the settings file.
		/// </param>
		private void SerializeSettings(string settingsFilePath, SettingsFile settings)
		{
			try
			{
				XmlSerializer xs = new XmlSerializer(typeof(SettingsFile));
				
				using (Stream s = File.OpenWrite(settingsFilePath))
				{
					xs.Serialize(s, settings);
				}
			}
			catch
			{
				throw;
			}
		}
		
		/// <summary>
		/// Gets the full path to the settings file.
		/// </summary>
		/// <returns>
		/// The full path to the settings file.
		/// </returns>
		private string GetSettingsFilePath()
		{
			string appDataDirectory = 
				Environment.GetFolderPath(
					Environment.SpecialFolder.CommonApplicationData);
			
			if (!appDataDirectory.EndsWith(@"\", StringComparison.OrdinalIgnoreCase))
		    {
		    	appDataDirectory += @"\";
		    }
			
			string settingsFileDirectory = 
				appDataDirectory + AppDataDirectoryName + @"\";
			string settingsFilePath = settingsFileDirectory + SettingsFileName;
			
			if (!Directory.Exists(settingsFileDirectory))
			{
				Directory.CreateDirectory(settingsFileDirectory);
			}

			return settingsFilePath;
		}		
		#endregion
		
		#region Event Functions
		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		/// <param name="disposing">
		/// [in] Disposing flag.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// Adds any newly created files in the source directory to the file
		/// queue so the files can be moved.
		/// </summary>
		/// <param name="source">
		/// [in] The event source.
		/// </param>
		/// <param name="e">
		/// [in] Information associated with the file system change.
		/// </param>
		private void OnChanged(object source, FileSystemEventArgs e)
		{
			//Show that a file has been created, changed, or deleted.
			WatcherChangeTypes wct = e.ChangeType;
			
			if (e.ChangeType == WatcherChangeTypes.Created)
			{
				if (File.Exists(e.FullPath))
				{
					// Store both the source and destination paths
					string sourcePath = 
						((Location)this.FileLocations[0]).Source;
					string destinationPath = 
						((Location)this.FileLocations[0]).Destination;
					
					// Get the list of folder name replacements
					List<Replacement> replacements = 
						((Location)this.FileLocations[0]).Replacements;
					
					// Create the new file path but substituting the source
					// folder with the destination folder
					string newPath = 
						e.FullPath.Replace(
							sourcePath,
							destinationPath);
					
					// Loop through the replacement folder names and replace any
					// source folders names with the destination folder names					
					foreach (Replacement replacement in replacements)
					{
						newPath = newPath.Replace(
							replacement.SourceName,
							replacement.DestinationName);
					}
					
					// Enqueue the file to be moved
					this.stagingFilesQueue.Enqueue(new StagingFile(
						e.FullPath,
						newPath));
				}
			}
		}
		
		/// <summary>
		/// Start this service.
		/// </summary>
		/// <param name="args">
		/// [in] Array of arguments to pass into the service.
		/// </param>
		protected override void OnStart(string[] args)
		{
			this.Initialize();
		}
		
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			this.Deinitialize();
		}
		
		/// <summary>
		/// Process the source directory cleanup.
		/// </summary>
		/// <param name="source">
		/// [in] The event source.
		/// </param>
		/// <param name="e">
		/// [in] Information associated with the elapsed time.
		/// </param>
		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			ProcessDirectory(((Location)this.FileLocations[0]).Source);
		}				
		#endregion
	}
}
