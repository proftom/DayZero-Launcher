using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Dotjosh.DayZCommander.App.Core;
using Dotjosh.DayZCommander.Updater;
using NLog;

namespace Dotjosh.DayZCommander.App
{
	public partial class App : Application
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public static EventAggregator Events = new EventAggregator();
		private bool _isUncaughtUiThreadException;

		protected override void OnStartup(StartupEventArgs e)
		{
			AppDomain.CurrentDomain.UnhandledException += UncaughtThreadException;
			DispatcherUnhandledException += UncaughtUiThreadException;

			ApplyUpdateIfNeccessary();

			LocalMachineInfo.Current.Update();

			base.OnStartup(e);
		}

		public static void ApplyUpdateIfNeccessary()
		{
			var thisLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var pendingUpdateDirectory = Path.Combine(thisLocation, DownloadAndExtracter.PENDING_UPDATE_DIRECTORYNAME);
			if(Directory.Exists(pendingUpdateDirectory))
			{
				var tempDir = DownloadAndExtracter.GetTempPath() + Guid.NewGuid();
				Directory.CreateDirectory(tempDir);
				CopyAllFiles(pendingUpdateDirectory, tempDir);
				var p = new Process()
				            {
				                StartInfo = new ProcessStartInfo()
				                   		        {
				                   		        	CreateNoWindow = false,
				                   		        	UseShellExecute = true,
				                   		        	Arguments = "\"" + thisLocation + "\"",
													WorkingDirectory = tempDir,
				                   		        	FileName = Path.Combine(tempDir, "DayZCommanderUpdater.exe")
				                   		        }
				            };
				p.Start();
				Environment.Exit(0);
			}
		}

		private static void CopyAllFiles(string sourcePath, string destinationPath)
		{
			foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
			{
				var destFile = new FileInfo(newPath.Replace(sourcePath, destinationPath));
				if(!destFile.Directory.Exists)
					destFile.Directory.Create();
				File.Copy(newPath, destFile.FullName);
			}
		}

		private void UncaughtUiThreadException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			_isUncaughtUiThreadException = true;
			_logger.Fatal(e.Exception);
//			var messageBoxResult = MessageBox.Show(
//				"It wasn't your fault, but something went really wrong!\r\nWould you like me to try and restart DayZ Commander for you?",
//				"Oh noes",
//				MessageBoxButton.YesNo);
//			if (messageBoxResult == MessageBoxResult.Yes)
//			{
//				e.Handled = true;
//				//System.Windows.Forms.Application.Restart();
//				Current.Shutdown();
//			}
		}

		private void UncaughtThreadException(object sender, UnhandledExceptionEventArgs e)
		{
			if(_isUncaughtUiThreadException)
				return;
			var exception = e.ExceptionObject as Exception;
			_logger.Fatal(exception);
		}
	}
}
