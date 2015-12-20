using System;

namespace TE.Apps.Staging
{
	/// <summary>
	/// Information related to a single staging file.
	/// </summary>
	public class StagingFile
	{
		#region Properties
		/// <summary>
		/// Gets or sets the source path.
		/// </summary>
		public string SourcePath { get; set; }
		
		/// <summary>
		/// Gets or sets the destination path.
		/// </summary>
		public string DestinationPath { get; set; }
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
		public StagingFile(string sourcePath, string destinationPath)
		{
			this.SourcePath = sourcePath;
			this.DestinationPath = destinationPath;
		}
		#endregion
	}
}
