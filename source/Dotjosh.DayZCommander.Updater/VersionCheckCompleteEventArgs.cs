using System;

namespace Dotjosh.DayZCommander.Updater
{
	public class VersionCheckCompleteEventArgs : EventArgs
	{
		public Version Version { get; set; }
		public bool IsNew { get; set; }
	}
}