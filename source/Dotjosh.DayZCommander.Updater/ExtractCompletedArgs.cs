using System;

namespace Dotjosh.DayZCommander.Updater
{
	public class ExtractCompletedArgs : EventArgs
	{
		public string TempExtractedLocation { get; set; }
	}
}