using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using zombiesnu.DayZeroLauncher.InstallUtilities;
using NLog;

namespace zombiesnu.DayZeroLauncher.Updater
{
	/// <summary>
	/// 	Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public MainWindow()
		{
			InitializeComponent();

			Loaded += OnLoaded;
		}

		private static void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			try
			{
				ApplyUpdate();
			}
			catch(Exception ex)
			{
				_logger.Error(ex);
				LaunchDayZeroLauncher();
				Environment.Exit(0);
			}
		}

		private static void ApplyUpdate()
		{
			var installDirectory = App.ApplicationInstallDirectory;
			var pendingUpdateDirectory = Path.Combine(installDirectory, DownloadAndExtracter.PENDING_UPDATE_DIRECTORYNAME);

			var currentDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
			var windowsTemp = new DirectoryInfo(Path.GetTempPath());
			var oldWindowsTempDirectoryStyle = currentDirectory.FullName.IndexOf(windowsTemp.FullName, StringComparison.CurrentCultureIgnoreCase) > -1;
			if(oldWindowsTempDirectoryStyle)
			{
				var tempUpdatePath = string.Format("{0}{1}", Path.GetTempPath(), Guid.NewGuid());
				var lastVersionPath = string.Format("{0}{1}", Path.GetTempPath(), Guid.NewGuid());

				KillDayZeroLauncherProcesses();
				Directory.Move(pendingUpdateDirectory, tempUpdatePath);
				Directory.Move(installDirectory, lastVersionPath);
				Directory.Move(tempUpdatePath, installDirectory);
			}
			else
			{
				var tempDirectory = Path.Combine(new DirectoryInfo(installDirectory).Parent.FullName, @"Temp\");

				var tempUpdatePath = string.Format("{0}{1}", tempDirectory, Guid.NewGuid());
				var lastVersionPath = string.Format("{0}{1}", tempDirectory, Guid.NewGuid());

				KillDayZeroLauncherProcesses();
				Directory.Move(pendingUpdateDirectory, tempUpdatePath);
				Directory.Move(installDirectory, lastVersionPath);
				Directory.Move(tempUpdatePath, installDirectory);
				try
				{
					Directory.Delete(tempDirectory, true);
				}
				catch (Exception)
				{
				}
			}
			UpdateShortcuts(Directory.GetParent(installDirectory).FullName);
			LaunchDayZeroLauncher();
		}

		private static void LaunchDayZeroLauncher()
		{
			var p = new Process
			        	{
			        		StartInfo = new ProcessStartInfo
			        		            	{
			        		            		CreateNoWindow = false,
			        		            		UseShellExecute = true,
			        		            		WorkingDirectory = App.ApplicationInstallDirectory,
			        		            		FileName = Path.Combine(App.ApplicationInstallDirectory, "DayZeroLauncher.exe")
			        		            	}
			        	};
			p.Start();
			Environment.Exit(0);
		}

		private static void KillDayZeroLauncherProcesses()
		{
			//Give it 10 times to kill all the dayz processes
			for(var i = 0; i < 10; i++)
			{
				try
				{
					var processes = Process.GetProcessesByName("DayZeroLauncher");
					if(processes.Length == 0)
					{
						return;
					}

					foreach(var process in processes)
					{
						process.Kill();
					}

					processes = Process.GetProcessesByName("DayZeroLauncher");
					if(processes.Length == 0)
					{
						return;
					}

					if(i == 9)
					{
						_logger.Log(LogLevel.Error, "Could not apply update, the DayZeroLauncher process could not be closed.");
					}

					Thread.Sleep(100);
				}
				// ReSharper disable EmptyGeneralCatchClause
				catch
				{
				}
				// ReSharper restore EmptyGeneralCatchClause
			}
		}

		private static void UpdateShortcuts(string installDirectory)
		{
			var installActions = new InstallActions
			                     	{
			                     		InstallDirectory = installDirectory
			                     	};
			installActions.UpdateShortcuts();
		}
	}
}