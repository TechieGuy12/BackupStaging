using System;
using System.IO;

namespace TE.Apps.Staging
{
	/// <summary>
	/// Information related to a single staging file.
	/// </summary>
	public class StagingFile
	{
		#region Private Variables
		/// <summary>
		/// The source path.
		/// </summary>
		private string _sourcePath;
		/// <summary>
		/// The destination path.
		/// </summary>
		private string _destinationPath;
		#endregion
		
		#region Properties
		/// <summary>
		/// Gets or sets the source path.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when the source path is null.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// Thrown when the source path is empty.
		/// </exception>
		public string SourcePath
		{
			get
			{
				return this._sourcePath;	
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(value);
				}
				
				if (value.Trim().Length == 0)
				{
					throw new ArgumentException(
						"The source path is empty.");
				}
				this._sourcePath = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the destination path.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when the destination path is null.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// Thrown when the destination path is empty.
		/// </exception>
		public string DestinationPath
		{
			get
			{
				return this._destinationPath;	
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(value);
				}
				
				if (value.Trim().Length == 0)
				{
					throw new ArgumentException(
						"The destination path is empty.");
				}
				this._destinationPath = value;
			}
		}			
		#endregion
		
		#region Constructors
		/// <summary>
		/// Creates a new instance of the <see cref="TE.Apps.Staging.StagingFile"/>
		/// class when provided with the source and destination paths.
		/// </summary>
		/// <param name="sourcePath">
		/// [in] The source path of the file.
		/// </param>
		/// <param name="destinationPath">
		/// [in] The destination path of the file.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when a path is null.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// Thrown when a path is empty.
		/// </exception>
		public StagingFile(string sourcePath, string destinationPath)
		{
			try
			{
				this.SourcePath = sourcePath;
				this.DestinationPath = destinationPath;
			}
			catch
			{
				throw;
			}
		}
		#endregion
		
		#region Public Functions
		/// <summary>
		/// Moves the file from the source directory to the destination
		/// directory.
		/// </summary>
		public void Move()
		{			
			// Check to see if the file exists before attempting to
			// move it to the destination
			if (File.Exists(this.SourcePath))
			{					
				string destinationDir = 
					Path.GetDirectoryName(this.DestinationPath);
				
				// If the destination directory doesn't exist, create
				// create it to avoid any exceptions						
				if (!Directory.Exists(destinationDir))
				{
					Directory.CreateDirectory(destinationDir);
				}
	
				try
				{
					// Check to see if the file exists in the
					// destination location
					if (File.Exists(this.DestinationPath))
					{
						// Copy the file, overwriting the destination
						// file and then delete the source file to
						// simulate a move
						File.Copy(
							this.SourcePath, 
							this.DestinationPath, 
							true);
						File.Delete(this.SourcePath);
					}
					else
					{
						// Move the file to the destination
						File.Move(
							this.SourcePath,
							this.DestinationPath);
					}													
				}
				catch
				{
					throw;								
				}
			}			
		}
		#endregion
	}
}
