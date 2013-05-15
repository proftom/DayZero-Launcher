using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using NLog;

// ReSharper disable InconsistentNaming
namespace zombiesnu.DayZeroLauncher.App.Core
{
	public class LocalMachineInfo : BindableBase
	{
		private static LocalMachineInfo _current;
		public static LocalMachineInfo Current
		{
			get
			{
				if(_current == null)
				{
					_current = new LocalMachineInfo();
					_current.Update();
				}
				return _current;
			}
		}

		public Version DayZeroLauncherVersion
		{
			get { return Assembly.GetEntryAssembly().GetName().Version; }
		}

		private string _arma2Path;
		public string Arma2Path
		{
			get { return _arma2Path; }
			private set
			{
				_arma2Path = value;
				PropertyHasChanged("Arma2Path");
			}
		}

		private string _arma2OaPath;
		public string Arma2OAPath
		{
			get { return _arma2OaPath; }
			private set
			{
				_arma2OaPath = value;
				PropertyHasChanged("Arma2OAPath");
			}
		}

		private string _steamPath;
		public string SteamPath
		{
			get { return _steamPath; }
			private set
			{
				_steamPath = value;
				PropertyHasChanged("SteamPath");
			}
		}

		private string _arma2OaBetaPath;
		public string Arma2OABetaPath
		{
			get { return _arma2OaBetaPath; }
			private set
			{
				_arma2OaBetaPath = value;
				PropertyHasChanged("Arma2OABetaPath");
			}
		}

		private string _arma2OaBetaExe;
		public string Arma2OABetaExe
		{
			get { return _arma2OaBetaExe; }
			private set
			{
				_arma2OaBetaExe = value;
				PropertyHasChanged("Arma2OABetaExe");
			}
		}

		private Version _arma2OaBetaVersion;
		public Version Arma2OABetaVersion
		{
			get { return _arma2OaBetaVersion; }
			private set
			{
				_arma2OaBetaVersion = value;
				PropertyHasChanged("Arma2OABetaVersion");
			}
		}

		private string _dayZPath;
		public string DayZPath
		{
			get { return _dayZPath; }
			private set
			{
				_dayZPath = value;
				PropertyHasChanged("DayZPath");
			}
		}

		private Version _dayZVersion;
		public Version DayZVersion
		{
			get { return _dayZVersion; }
			private set
			{
				_dayZVersion = value;
				PropertyHasChanged("DayZVersion");
			}
		}

		public void Update()
		{
			try
			{
				if(IntPtr.Size == 8)
				{
					SetPathsX64();
				}
				else
				{
					SetPathsX86();
				}

                Arma2OABetaVersion = GameVersions.ExtractArma2OABetaVersion(Arma2OABetaExe);
                DayZVersion = GameVersions.ExtractDayZVersion(DayZPath);
			}
			catch//(Exception e)
			{
				//Disabled for now
				//_logger.ErrorException("Unable to retrieve Local Machine Info.", e);
			}
		}

		private void SetPathsX64()
		{
			const string arma2Registry = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2";
			const string arma2OARegistry = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Bohemia Interactive Studio\ArmA 2 OA";
			const string steamRegistry = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam";

			SetPaths(arma2Registry, arma2OARegistry, steamRegistry);
		}

		private void SetPathsX86()
		{
			const string arma2Registry = @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2";
			const string arma2OARegistry = @"HKEY_LOCAL_MACHINE\SOFTWARE\Bohemia Interactive Studio\ArmA 2 OA";
			const string steamRegistry = @"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam";

			SetPaths(arma2Registry, arma2OARegistry, steamRegistry);
		}

		private void SetPaths(string arma2Registry, string arma2OARegistry, string steamRegistry)
		{
			// Set game paths.
			Arma2Path = (string)Registry.GetValue(arma2Registry, "main", "");
			Arma2OAPath = (string)Registry.GetValue(arma2OARegistry, "main", "");
			SteamPath = (string)Registry.GetValue(steamRegistry, "InstallPath", "");

			// If a user does not run a game the path will be null.
			if(string.IsNullOrWhiteSpace(Arma2Path)
				&& !string.IsNullOrWhiteSpace(Arma2OAPath))
			{
				var pathInfo = new DirectoryInfo(Arma2OAPath);
				if(pathInfo.Parent != null)
				{
					Arma2Path = Path.Combine(pathInfo.Parent.FullName, "arma 2");
				}
			}
			if(!string.IsNullOrWhiteSpace(Arma2Path)
				&& string.IsNullOrWhiteSpace(Arma2OAPath))
			{
				var pathInfo = new DirectoryInfo(Arma2Path);
				if(pathInfo.Parent != null)
				{
					Arma2OAPath = Path.Combine(pathInfo.Parent.FullName, "arma 2 operation arrowhead");
				}
			}

			if(string.IsNullOrWhiteSpace(Arma2OAPath))
			{
				return;
			}

			Arma2OABetaPath = Path.Combine(Arma2OAPath, @"Expansion\beta");
			Arma2OABetaExe = Path.Combine(Arma2OABetaPath, @"arma2oa.exe");
			DayZPath = Path.Combine(Arma2OAPath, @"@DayZero");
		}
	}
}