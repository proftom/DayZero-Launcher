using System;
using System.IO;

namespace zombiesnu.DayZeroLauncher.App.Core
{
	public class CalculatedGameSettings : BindableBase
	{
		private static CalculatedGameSettings _current;
		public static CalculatedGameSettings Current
		{
			get
			{
				if(_current == null)
				{
					_current = new CalculatedGameSettings();
					_current.Update();
				}
				return _current;
			}
		}

		public string Arma2Path { get; set; }
		public string Arma2OAPath { get; set; }
		public string Arma2OAExePath { get; set; }
		public string DayZPath { get; set; }
		public Version Arma2OABetaVersion { get; set; }
		public Version DayZVersion { get; set; }

		public void Update()
		{
			SetArma2Path();
			SetArma2OAPath();
			SetArma2OAExePath();
			SetDayZPath();
			SetArma2OABetaVersion();
			SetDayZVersion();
			PropertyHasChanged("Arma2Path", "Arma2OAPath", "Arma2OAExePath", "DayZPath", "Arma2OABetaVersion", "DayZVersion");
		}

		public void SetArma2Path()
		{
			if (!string.IsNullOrWhiteSpace(UserSettings.Current.GameOptions.Arma2DirectoryOverride))
				Arma2Path = UserSettings.Current.GameOptions.Arma2DirectoryOverride;
			else
				Arma2Path = LocalMachineInfo.Current.Arma2Path;
		}

		public void SetArma2OAPath()
		{
			if (!string.IsNullOrWhiteSpace(UserSettings.Current.GameOptions.Arma2OADirectoryOverride))
				Arma2OAPath = UserSettings.Current.GameOptions.Arma2OADirectoryOverride;
			else
				Arma2OAPath = LocalMachineInfo.Current.Arma2OAPath;
		}

		private void SetArma2OAExePath()
		{
			if (!string.IsNullOrWhiteSpace(UserSettings.Current.GameOptions.Arma2OADirectoryOverride))
                Arma2OAExePath = GameVersions.BuildArma2OAExePath(UserSettings.Current.GameOptions.Arma2OADirectoryOverride);
			else
				Arma2OAExePath = LocalMachineInfo.Current.Arma2OABetaExe;
		}

		public void SetDayZPath()
		{
			if (!string.IsNullOrWhiteSpace(UserSettings.Current.GameOptions.DayZDirectoryOverride))
				DayZPath = UserSettings.Current.GameOptions.DayZDirectoryOverride;
			else
				DayZPath = LocalMachineInfo.Current.DayZPath;
		}

		private void SetArma2OABetaVersion()
		{
			if(!string.IsNullOrEmpty(Arma2OAExePath))
				Arma2OABetaVersion = GameVersions.ExtractArma2OABetaVersion(Arma2OAExePath);
			else
				Arma2OABetaVersion = null;
		}

		private void SetDayZVersion()
		{
			if(!string.IsNullOrEmpty(DayZPath))
				DayZVersion = GameVersions.ExtractDayZVersion(DayZPath);
			else
				DayZVersion = null;
		}
	}
}