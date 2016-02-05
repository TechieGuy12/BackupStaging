/*
 * Created by SharpDevelop.
 * User: Paul
 * Date: 12/19/2015
 * Time: 3:32 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
//		/// <summary>
//		/// Gets or sets the source directory.
//		/// </summary>
//		public string SourceDirectory { get; set; }
//		
//		/// <summary>
//		/// Gets or set the destination directory.
//		/// </summary>
//		public string DestinationDirectory { get; set; }
		
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
			this.Locations = new List<Location>();
			this.MoveRetryCount = 5;
			this.MoveRetryWait = 30;
		}
		#endregion
	}
}
