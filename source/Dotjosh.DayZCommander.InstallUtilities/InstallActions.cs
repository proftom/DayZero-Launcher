using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;

namespace Dotjosh.DayZCommander.InstallUtilities
{
	[RunInstaller(true)]
	public partial class InstallActions : Installer
	{
		private const string _mainExecutable = "DayZCommander.exe";

		public InstallActions()
		{
			InitializeComponent();
		}

		public string InstallDirectory { get; set; }

		public override void Install(IDictionary stateSaver)
		{
			//MessageBox.Show("attach install");
			base.Install(stateSaver);
			InstallDirectory = Context.Parameters["targetdir"].Replace("|", "");
			CreateShortcuts();
		}

		public override void Uninstall(IDictionary savedState)
		{
			//MessageBox.Show("attach uninstall");
			DeleteShortcuts();
			base.Uninstall(savedState);
		}

		public void UpdateShortcuts()
		{
			if(DeleteShortcut(InstallDirectory))
			{
				CreateDayZCommanderShortcut(InstallDirectory);
			}
			if(DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)))
			{
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
			}
			if(DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)))
			{
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
			}
			if(DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)))
			{
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
			}
			if(DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)))
			{
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
			}
		}

		private void CreateShortcuts()
		{
			var allUsers = Context.Parameters["allusers"];
			CreateDayZCommanderShortcut(InstallDirectory);
			if(!string.IsNullOrEmpty(allUsers)
				&& allUsers.Equals("1"))
			{
				// Everyone.
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
			}
			else
			{
				// Just me.
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
				CreateDayZCommanderShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
			}
		}

		private static void DeleteShortcuts()
		{
			DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory));
			DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
			DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu));
			DeleteShortcut(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
		}

		private void CreateDayZCommanderShortcut(string shortcutPath)
		{
			var workingDirectory = Path.Combine(InstallDirectory, @"Current");
			var targetPath = Path.Combine(workingDirectory, _mainExecutable);
			var shortcutFile = Path.Combine(shortcutPath, "DayZ Commander.lnk");
			CreateShortcut(targetPath, workingDirectory, "DayZ Commander", shortcutFile);
		}

		private static void CreateShortcut(string target, string workingDirectory, string description, string shortcutFile)
		{
			using(var shortcut = new ShellLink())
			{
				shortcut.Target = target;
				shortcut.WorkingDirectory = workingDirectory;
				shortcut.Description = description;
				shortcut.DisplayMode = ShellLink.LinkDisplayMode.edmNormal;
				shortcut.Save(shortcutFile);
			}
		}

		private static bool DeleteShortcut(string shortcutPath)
		{
			var shortcutFile = Path.Combine(shortcutPath, "DayZ Commander.lnk");
			if(!File.Exists(shortcutFile))
			{
				return false;
			}
			File.Delete(shortcutFile);
			return true;
		}
	}
}