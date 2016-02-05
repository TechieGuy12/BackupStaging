/*
 * Created by SharpDevelop.
 * User: Paul
 * Date: 1/23/2016
 * Time: 10:33 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace TE.Apps.Staging
{
	/// <summary>
	/// The source and destination location of the files.
	/// </summary>
	public class Location
	{
		#region Properties
		/// <summary>
		/// Gets or sets the source directory.
		/// </summary>
		public string Source { get; set; }
		
		/// <summary>
		/// Gets or sets the destination directory.
		/// </summary>
		public string Destination { get; set; }
		
		/// <summary>
		/// Gets or sets the list of replacement directory names.
		/// </summary>
		public List<Replacement> Replacements { get; set; }
		#endregion
		
		#region Constructors
		private Location() {}
		
		/// <summary>
		/// Creates a new instance of the <see cref="TE.Apps.Staging.Location"/>
		/// class when provided with the source and destination directory names.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">
		/// A parameter is null or empty.
		/// </exception>
		public Location(string source, string destination)
		{
			if (string.IsNullOrEmpty(source))
			{
				throw new ArgumentNullException(
					source, 
					"The source directory cannot be null or empty.");
			}
			
			if (string.IsNullOrEmpty(destination))
			{
				throw new ArgumentNullException(
					destination, 
					"The destination directory cannot be null or empty.");
			}
			
			this.Source = source;
			this.Destination = destination;
			this.Replacements = new List<Replacement>();
		}
		#endregion
	}
}
