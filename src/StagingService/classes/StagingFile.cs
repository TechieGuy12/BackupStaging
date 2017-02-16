using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TE.Apps.Staging
{
	/// <summary>
	/// Information related to a single staging file.
	/// </summary>
	public class StagingFile
	{
		#region Properties
		/// <summary>
		/// Gets the source path.
		/// </summary>
		public string SourcePath { get; private set; }
		
		/// <summary>
		/// Gets the destination path.
		/// </summary>
		public string DestinationPath { get; private set; }			
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
		/// Thrown when a path is null or empty.
		/// </exception>
		public StagingFile(string sourcePath, string destinationPath)
		{
			if (string.IsNullOrEmpty(sourcePath))
			{
				throw new ArgumentNullException(
					sourcePath,
					"The source path parameter cannot be null.");
			}
			
			if (string.IsNullOrEmpty(destinationPath))
			{
				throw new ArgumentNullException(
					destinationPath,
					"The destination path parameter cannot be null.");
			}
			
			SourcePath = sourcePath;
			DestinationPath = destinationPath;
		}
		#endregion
		
        /// <summary>
        /// Gets the unique hash that represents the full path of the file.
        /// </summary>
        /// <param name="filePath">
        /// The full path of the file to be hashed.
        /// </param>
        /// <returns>
        /// The SHA256 hash of the path or a null string if no hash could be
        /// generated.
        /// </returns>
        private string GetFilePathHash(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            using (var sha = new SHA256CryptoServiceProvider())
            {
                // Set the encoding to UTF8 and computer the hash
                Encoding enc = Encoding.UTF8;
                byte[] hash = sha.ComputeHash(enc.GetBytes(filePath));

                // Verify the hash length is greater than zero, otherwise
                // return a null string
                if (hash.Length > 0)
                {
                    // Return the file hash
                    return BitConverter.ToString(hash).Replace("-", string.Empty);;
                }
                else
                {
                    return null;
                }

            }
        }
        
        /// <summary>
        /// Checks to see if the file hash for a source and destination file
        /// are the equal.
        /// </summary>
        /// <param name="source">
        /// The full path to the source file.
        /// </param>
        /// <param name="destination">
        /// The full path to the destination file.
        /// </param>
        /// <returns>
        /// True if the file hashes are the same, false if they are not the
        /// same.
        /// </returns>
        private bool IsHashesEqual(string source, string destination)
        {
        	// If either the source or destination parameters are null or
        	// empty, then return false
        	if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination))
        	{
        		return false;
        	}
        	
        	// If either the source or destination files don't exist, then
        	// return false
        	if (!File.Exists(source) || !File.Exists(destination))
        	{
        		return false;
        	}
        	
        	// Get the source and destination hashes
        	string sourceHash = GetFilePathHash(source);
        	string destinationHash = GetFilePathHash(destination);
        	
        	// Return the value indicating if the source and destination hashes
        	// are equal
        	return sourceHash.Equals(destinationHash);
        }
        
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
					// Copy the file, overwriting the destination
					// file and then delete the source file to
					// simulate a move
					File.Copy(
						SourcePath, 
						DestinationPath, 
						true);
					
					if (IsHashesEqual(SourcePath, DestinationPath))
					{
						File.Delete(SourcePath);										
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
