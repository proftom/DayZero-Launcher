using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace Dotjosh.DayZCommander.Updater
{
	public class VersionChecker
	{
		private Uri _versionUri;
		private Version _thisVersion;

		public VersionChecker()
		{
			_versionUri = new Uri("http://files.dayzcommander.com/releases/versioninfo.txt");
			_thisVersion = Assembly.GetEntryAssembly().GetName().Version;
		}

		public event EventHandler<VersionCheckCompleteEventArgs> Complete;

		public void CheckForUpdate()
		{
			var checkForUpdateClient = new WebClient();
			checkForUpdateClient.DownloadStringCompleted += DownloadVersionInfoCompleted;
			checkForUpdateClient.DownloadStringAsync(_versionUri);
		}

		private void DownloadVersionInfoCompleted(object sender, DownloadStringCompletedEventArgs args)
		{
			Version serverVersion;
			if(args.Error != null)
			{
				OnComplete(null, false);
				return;
			}
			if(Version.TryParse(args.Result, out serverVersion))
			{
				//var isNew = serverVersion > _thisVersion;
                var isNew = false;
				OnComplete(serverVersion, isNew);
				return;
			}
			OnComplete(null, false);
		}

		private void OnComplete(Version newVersion, bool isNew)
		{
			if(Complete != null)
			{
				var args = new VersionCheckCompleteEventArgs
				           	{
				           		Version = newVersion,
				           		IsNew = isNew
				           	};
				Complete(this, args);
			}
		}
	}
}