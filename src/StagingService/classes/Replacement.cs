using System;

namespace TE.Apps.Staging
{
	/// <summary>
	/// Description of Replacement.
	/// </summary>
	public class Replacement
	{
		#region Properties
		/// <summary>
		/// Gets or sets the source directory.
		/// </summary>
		public string SourceName { get; set; }
		
		/// <summary>
		/// Gets or set the destination directory.
		/// </summary>
		public string DestinationName { get; set; }			
		#endregion
		
		#region Constructors
		/// <summary>
		/// Creates a new instance of the <see cref="TE.Apps.Staging.Replacement"/>
		/// class
		/// </summary>
		private Replacement() {}
		
		/// <summary>
		/// Creates a new instance of the <see cref="TE.Apps.Staging.Replacement"/>
		/// class when provided with the source and destination directories.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">
		/// A parameter is null or empty.
		/// </exception>
		public Replacement(string sourceName, string destinationName)
		{
			if (string.IsNullOrEmpty(sourceName))
			{
				throw new ArgumentNullException(
					sourceName, 
					"The source name cannot be null or empty.");
			}
			
			if (string.IsNullOrEmpty(destinationName))
			{
				throw new ArgumentNullException(
					destinationName, 
					"The destination name cannot be null or empty.");
			}

			SourceName = sourceName;
			DestinationName = destinationName;
		}
		#endregion
	}
}
