using System;

namespace zombiesnu.DayZeroLauncher.Updater
{
	public class ExtractCompletedArgs : EventArgs
	{
		public string TempExtractedLocation { get; set; }
	}
}