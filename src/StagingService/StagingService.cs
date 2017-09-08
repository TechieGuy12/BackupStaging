using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        /// The number of threads to use to move the files.
        /// </summary>
        private int threads;
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
		private ConcurrentQueue<StagingFile> stagingFilesQueue;
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
			bg.CancelAsync();
			bg.Dispose();
			
			timer.Enabled = false;
			timer.Close();
			timer.Dispose();
			
			watcher.EnableRaisingEvents = false;
			watcher.Dispose();
			
			bg = null;
			timer = null;
			watcher = null;
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
				return (SettingsFile)xs.Deserialize(s);
			}
		}
		

        /// <summary>
        /// Gets the number of threads specified in the settings file.
        /// </summary>
        /// <returns>
        /// The number of threads to use to move the files.
        /// </returns>
        private int GetThreads()
        {
            /// The number of processors
            int processors = Environment.ProcessorCount;
            // The actual number of threads to use
            int actualThreads = 1;
            // The expected threads as specified in the settings file
            int expectedThreads = settingsFile.Threads;

            // Check to see if the value is indicating a value that is relative
            // to the number of processors - for example: -1 would indicate
            // that the threads should be 1 less than the number or processors
            if (expectedThreads < 1)
            {
                // If the expected threads value is greater than then number
                // of processors, then default to one thread since you can't
                // have less than 0 threads
                if (Math.Abs(expectedThreads) >= processors)
                {
                    actualThreads = 1;
                }
                else
                {
                    // Otherwise calculate the number of threads by adding the
                    // negative number to the processor count
                    actualThreads = processors + expectedThreads;
                }
            }
            else
            {
                // If the expected threads is greater than the number of
                // processors, then default the number of threads to equal
                // the number of processors
                if (expectedThreads > processors)
                {
                    actualThreads = processors;
                }
                else
                {
                    // Just set the number of threads to the value specified
                    // in the settings file
                    actualThreads = expectedThreads;
                }
            }

            Logging.WriteLine("Number of threads to use: " + actualThreads.ToString());

            return actualThreads;

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
				settings.MoveRetryCount = 5;
				settings.MoveRetryWait = 30;
				
				// Save the settings to the file
				SerializeSettings(settingsFilePath, settings);
			}
			
			// Read the settings from the file
			return DeserializeSettings(settingsFilePath);
		}
		
		/// <summary>
		/// Initializes objects when the service is started.
		/// </summary>
		private void Initialize()
		{
            Logging.Delete();
            settingsFile = new SettingsFile();
			settingsFile = GetSettings();

            threads = GetThreads();

			// Initialize the properties			
			FileLocations = settingsFile.Locations;
			RetryMoveCount = settingsFile.MoveRetryCount;
			RetryMoveWait = settingsFile.MoveRetryWait;
			
			// Initialize the file move queue
			stagingFilesQueue = new ConcurrentQueue<StagingFile>();
			
			// Initialize the BackgroundWorker object that will perform the
			// moving of the staging files
			bg = new BackgroundWorker();			
			bg.WorkerSupportsCancellation = true;
			bg.DoWork += new DoWorkEventHandler(MoveStagingFiles);
			bg.RunWorkerAsync();

			// Initialize the timer that will cleanup any empty directories
			// in the staging directory
			timer = new System.Timers.Timer();
			timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			timer.Interval = 10000;
			timer.Enabled = true;			
			
			// Initialize the FileSystemWatcher object to watch the staging
			// directory for changes
			watcher = new FileSystemWatcher();
			watcher.Path = ((Location)FileLocations[0]).Source;
			watcher.IncludeSubdirectories = true;
			watcher.NotifyFilter = 
				NotifyFilters.LastAccess | 
				NotifyFilters.LastWrite | 
				NotifyFilters.FileName | 
				NotifyFilters.DirectoryName;
			watcher.Filter = "*.*";
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			watcher.Created += new FileSystemEventHandler(OnChanged);
			watcher.EnableRaisingEvents = true;		
		}

		/// <summary>
		/// Initializes objects when the service created.
		/// </summary>
		private void InitializeComponent()
		{
			ServiceName = MyServiceName;
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
					
					// Delete folders that were last written to more than 10 minutes ago
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
			if (bg == null || stagingFilesQueue == null)
			{
				return;
			}
			
			while (!bg.CancellationPending)
			{
                // If there are no files in the queue, then sleep for 100ms
				if (stagingFilesQueue.IsEmpty)
				{
                    System.Threading.Thread.Sleep(100);
				}
				else
				{                    
                    Parallel.ForEach(
                        stagingFilesQueue,
                        new ParallelOptions
                        {
                            MaxDegreeOfParallelism = threads
                        },
                        q =>
                        {
                            // Get the next file in the queue
                            StagingFile stagingFile = null;
                            stagingFilesQueue.TryDequeue(out stagingFile);

                            int i = 0;
                            while (i < RetryMoveCount)
                            {
                                try
                                {
                                    stagingFile.Move();
                                    i = RetryMoveCount;
                                }
                                catch (Exception ex) when (ex is IOException || ex is FilesNotEqualException)
                                {
                                    // Wait for a specified number of milliseconds
                                    // and then increment the retry counter
                                    Thread.Sleep(RetryMoveWait * 1000);
                                    i++;

                                    // If the retry copy count reaches the limit,
                                    // then re-enqueue the file to try again later
                                    if (i == RetryMoveCount)
                                    {
                                        stagingFilesQueue.Enqueue(stagingFile);
                                    }
                                }
                            }
                        });
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
			
			SerializeSettings(settingsFilePath, settings);

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
			
            string settingsFileDirectory = Path.Combine(appDataDirectory, AppDataDirectoryName);
            string settingsFilePath = Path.Combine(settingsFileDirectory, SettingsFileName);
			
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
						((Location)FileLocations[0]).Source;
					string destinationPath = 
						((Location)FileLocations[0]).Destination;
					
					// Get the list of folder name replacements
					List<Replacement> replacements = 
						((Location)FileLocations[0]).Replacements;
					
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
					stagingFilesQueue.Enqueue(new StagingFile(
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
			Initialize();
		}
		
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			Deinitialize();
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
			ProcessDirectory(((Location)FileLocations[0]).Source);
		}				
		#endregion
	}
}
