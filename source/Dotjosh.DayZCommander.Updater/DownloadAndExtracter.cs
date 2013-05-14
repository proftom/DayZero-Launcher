using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Ionic.Zip;
using NLog;

namespace Dotjosh.DayZCommander.Updater
{
	public class DownloadAndExtracter
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly Version _serverVersion;
		private readonly Uri _serverZipUri;
		private readonly string _tempDownloadFileLocation;
		private readonly string _tempExtractedLocation;
		public static readonly string PENDING_UPDATE_DIRECTORYNAME = "_pendingupdate";
		private readonly string _currentDirectory;
		private readonly string _targetSwapDirectory;

		public DownloadAndExtracter(Version serverVersion)
		{
			_serverVersion = serverVersion;
			_serverZipUri = new Uri(String.Format("http://files.dayzcommander.com/releases/{0}.zip", _serverVersion));
			var uniqueToken = Guid.NewGuid().ToString();
			_tempDownloadFileLocation = DownloadAndExtracter.GetTempPath() + uniqueToken + ".zip";
			_tempExtractedLocation = DownloadAndExtracter.GetTempPath() + uniqueToken;
			_currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			_targetSwapDirectory = Path.Combine(_currentDirectory, PENDING_UPDATE_DIRECTORYNAME);
		}

		public static string GetTempPath()
		{
			var current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var currentInfo = new DirectoryInfo(current);
			var tempPath = Path.Combine(currentInfo.Parent.FullName, @"Temp\");
			if(!Directory.Exists(tempPath))
				Directory.CreateDirectory(tempPath);
			return tempPath;
		}

		public string TempExtractedLocation
		{
			get { return _tempExtractedLocation; }
		}

		public event EventHandler<ExtractCompletedArgs> ExtractComplete;

		public void DownloadAndExtract()
		{
			var pendingUpdateVersion = GetPendingUpdateVersion();
			if(pendingUpdateVersion != null && pendingUpdateVersion >= _serverVersion)
			{
				OnExtractComplete();
				return;
			}

			var checkForUpdateClient = new WebClient();
			checkForUpdateClient.DownloadFileCompleted += DownloadFileComplete;
			checkForUpdateClient.DownloadFileAsync(_serverZipUri, _tempDownloadFileLocation);
		}

		private Version GetPendingUpdateVersion()
		{
			if(!Directory.Exists(_targetSwapDirectory))
			{
				return null;
			}
			var pendingUpdateDayZCommanderFile = new FileInfo(Path.Combine(_targetSwapDirectory, "DayZCommander.exe"));
			if(!pendingUpdateDayZCommanderFile.Exists)
				return null;

			return AssemblyName.GetAssemblyName(pendingUpdateDayZCommanderFile.FullName).Version;
		}

		private void DownloadFileComplete(object sender, AsyncCompletedEventArgs args)
		{
			if(args.Error != null)
			{
				return;
			}
			Extract();
		}

		private void Extract()
		{
			//Take advantage of async IO for the download, but start a thread for the extract and file operations
			new Thread(() =>
			           	{
							try
							{
								using (var zipFile = ZipFile.Read(_tempDownloadFileLocation))
								{
									zipFile.ExtractAll(_tempExtractedLocation, ExtractExistingFileAction.OverwriteSilently);
								}

								if (Directory.Exists(_targetSwapDirectory))
									Directory.Delete(_targetSwapDirectory);

								Directory.Move(_tempExtractedLocation, _targetSwapDirectory);

								Action action = OnExtractComplete;
								Application.Current.Dispatcher
									.BeginInvoke(action, DispatcherPriority.Background);
							}
							catch(Exception ex)
							{
								_logger.Error(ex);
							}
			           	}).Start();

		}

		private void OnExtractComplete()
		{
			if(ExtractComplete != null)
			{
				ExtractComplete(this, new ExtractCompletedArgs());
			}
		}
	}
}