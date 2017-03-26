using System;
using System.Collections.Generic;

namespace TE.Apps.Staging
{
	/// <summary>
	/// Information about the settings used by the service.
	/// </summary>
	public class SettingsFile
	{
		#region Properties	
		/// <summary>
		/// Gets or sets the list of source and destination locations.
		/// </summary>
		public List<Location> Locations { get; set; }
		
		/// <summary>
		/// Gets or sets the file move retry count.
		/// </summary>
		public int MoveRetryCount { get; set; }
		
		/// <summary>
		/// Gets or sets the file retry wait time in seconds.
		/// </summary>
		public int MoveRetryWait { get; set; }

        /// <summary>
        /// Gets or sets the threads to use to execute the copy.
        /// </summary>
        public int Threads { get; set; }
		#endregion
		
		#region Constructors
		/// <summary>
		/// Creates a new instance of the <see cref="TE.Apps.Staging.SettingsFile"/>
		/// class.
		/// </summary>
		public SettingsFile()
		{
			this.Initialize();
		}
		#endregion
		
		#region Private Functions
		/// <summary>
		/// Initializes the objects used by the class.
		/// </summary>
		private void Initialize()
		{
			Locations = new List<Location>();
			MoveRetryCount = 5;
			MoveRetryWait = 30;
            Threads = 1;
		}
		#endregion
	}
}
