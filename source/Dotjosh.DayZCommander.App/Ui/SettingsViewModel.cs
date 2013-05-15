using zombiesnu.DayZeroLauncher.App.Core;

namespace zombiesnu.DayZeroLauncher.App.Ui
{
	public class SettingsViewModel : ViewModelBase
	{
		private bool _isVisible;

		public SettingsViewModel()
		{
			Settings = UserSettings.Current;
		}

		public UserSettings Settings { get; set; }

		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				PropertyHasChanged("IsVisible");
			}
		}

		public string Arma2Directory
		{
			get
			{
				if(!string.IsNullOrWhiteSpace(Settings.GameOptions.Arma2DirectoryOverride))
				{
					return Settings.GameOptions.Arma2DirectoryOverride;
				}
				return LocalMachineInfo.Current.Arma2Path;
			}
			set
			{
				Settings.GameOptions.Arma2DirectoryOverride = value;
				PropertyHasChanged("Arma2Directory");
			}
		}

		public bool Arma2DirectoryOverride
		{
			get { return !string.IsNullOrWhiteSpace(Settings.GameOptions.Arma2DirectoryOverride); }
			set
			{
				if(value)
					Settings.GameOptions.Arma2DirectoryOverride = LocalMachineInfo.Current.Arma2Path ?? "Replace with full Arma2 Path";
				else
					Settings.GameOptions.Arma2DirectoryOverride = null;
				PropertyHasChanged("Arma2Directory", "Arma2DirectoryOverride");
			}
		}

		public string Arma2OADirectory
		{
			get
			{
				if(!string.IsNullOrWhiteSpace(Settings.GameOptions.Arma2OADirectoryOverride))
				{
					return Settings.GameOptions.Arma2OADirectoryOverride;
				}
				return LocalMachineInfo.Current.Arma2OAPath;
			}
			set
			{
				Settings.GameOptions.Arma2OADirectoryOverride = value;
				PropertyHasChanged("Arma2OADirectory", "Arma2OADirectoryOverride");
			}
		}

		public bool Arma2OADirectoryOverride
		{
			get { return !string.IsNullOrWhiteSpace(Settings.GameOptions.Arma2OADirectoryOverride); }
			set
			{
				if(value)
					Settings.GameOptions.Arma2OADirectoryOverride = LocalMachineInfo.Current.Arma2OAPath ?? "Replace with full Arma2 OA Path";
				else
					Settings.GameOptions.Arma2OADirectoryOverride = null;
				PropertyHasChanged("Arma2OADirectory", "Arma2OADirectoryOverride");
			}
		}

        public string DayZDirectory
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Settings.GameOptions.DayZDirectoryOverride))
                {
                    return Settings.GameOptions.DayZDirectoryOverride;
                }
                return LocalMachineInfo.Current.DayZPath;
            }
            set
            {
                Settings.GameOptions.DayZDirectoryOverride = value;
                PropertyHasChanged("DayZDirectory", "DayZDirectoryOverride");
            }
        }

        public bool DayZDirectoryOverride
        {
            get { return !string.IsNullOrWhiteSpace(Settings.GameOptions.DayZDirectoryOverride); }
            set
            {
                if (value)
                    Settings.GameOptions.DayZDirectoryOverride = LocalMachineInfo.Current.DayZPath ?? "Replace with full DayZ Path";
                else
                    Settings.GameOptions.DayZDirectoryOverride = null;
                PropertyHasChanged("DayZDirectory", "DayZDirectoryOverride");
            }
        }

		public void Done()
		{
			IsVisible = false;
		}
	}
}