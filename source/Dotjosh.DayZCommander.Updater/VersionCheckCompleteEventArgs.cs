using System;

namespace zombiesnu.DayZeroLauncher.Updater
{
	public class VersionCheckCompleteEventArgs : EventArgs
	{
		public Version Version { get; set; }
		public bool IsNew { get; set; }
	}
}