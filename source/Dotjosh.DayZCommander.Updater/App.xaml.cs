using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using NLog;

namespace Dotjosh.DayZCommander.Updater
{
	public partial class App
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if(e.Args.Length == 0)
			{
				MessageBox.Show("The DayZ Updater application should not be run manually.");
			}

			var path = e.Args[0];
			if(!Directory.Exists(path))
			{
				throw new ApplicationException(string.Format("Must provide a valid path to application, {0} does not exist", path));
			}

			ApplicationInstallDirectory = path;
		}

		public static string ApplicationInstallDirectory { get; set; }

	}
}
