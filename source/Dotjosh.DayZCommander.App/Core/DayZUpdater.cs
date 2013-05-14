using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace Dotjosh.DayZCommander.App.Core
{
	public class DayZUpdater : BindableBase
	{
		private Version _latestVersion;
		private bool _isChecking;
		private string _status;
		public const string DayZListingUrl = "http://cdn.armafiles.info/latest/";

		public DayZUpdater()
		{
			Installer = new DayZInstaller();
			Installer.PropertyChanged += (sender, args) =>
			                             	{
												if(args.PropertyName == "IsRunning")
												{
													PropertyHasChanged("InstallButtonVisible");
												}
												else if(args.PropertyName == "Status")
												{
													if(Installer.Status == "Install complete")
													{
														CheckForUpdates();
													}
												}
			                             	};
		}

		public DayZInstaller Installer { get; set; }

		public bool VersionMismatch
		{
			get
			{
				if(CalculatedGameSettings.Current.DayZVersion == null)
					return true;
				if(LatestVersion == null)
					return false;

                return !CalculatedGameSettings.Current.DayZVersion.Equals(LatestVersion);
			}
		}
	
		public void CheckForUpdates()
		{
			if(_isChecking)
				return;

			_isChecking = true;

			Status = DayZCommanderUpdater.STATUS_CHECKINGFORUPDATES;

			string responseBody;
			Version latestVersion = null;

			new Thread(() =>
			           	{
			           		try
			           		{
								Thread.Sleep(750);  //In case this happens so fast the UI looks like it didn't work
			           			if (!GameUpdater.HttpGet(DayZListingUrl, out responseBody))
			           			{
			           				Status = "cdn.armafiles.com not responding";
			           				return;
			           			}
			           			var latestCodeFileMatch = Regex.Match(responseBody,
			           			                                      @"<a\s+href\s*=\s*(?:'|"")(dayz_code_[^'""]+)(?:'|"")",
			           			                                      RegexOptions.IgnoreCase);
			           			if (!latestCodeFileMatch.Success)
			           			{
			           				Status = "Filenames don't match expected pattern";
			           				return;
			           			}
			           			var latestCodeFile = latestCodeFileMatch.Groups[1].Value;
			           			var latestCodeVersionMatch = Regex.Match(latestCodeFile, @"\d(?:\.\d){1,3}");
			           			if (!latestCodeVersionMatch.Success)
			           			{
			           				Status = "Could not determine version from filenames";
			           				return;
			           			}
			           			Version version;
			           			if (Version.TryParse(latestCodeVersionMatch.Value, out version))
			           			{
			           				latestVersion = version;
                                    if (!latestVersion.Equals(CalculatedGameSettings.Current.DayZVersion))
			           				{
			           					Status = DayZCommanderUpdater.STATUS_OUTOFDATE;
			           				}
			           				else
			           				{
			           					Status = DayZCommanderUpdater.STATUS_UPTODATE;
			           				}
			           			}
			           			else
			           			{
			           				Status = "Could not determine version from filenames";

			           			}
			           		}
			       			catch(Exception ex)
							{
								Status = "Error getting version";
							}
							finally
							{
								_isChecking = false;
								LatestVersion = latestVersion;
							}
						}).Start();
		}

		public Version LatestVersion
		{
			get { return _latestVersion; }
			set
			{
				_latestVersion = value;
				Execute.OnUiThread(() => PropertyHasChanged("LatestVersion", "VersionMismatch", "InstallButtonVisible"));			
			}
		}

		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				Execute.OnUiThread(() => PropertyHasChanged("Status", "VersionMismatch", "InstallButtonVisible"));
			}
		}

		public bool InstallButtonVisible
		{
			get { return VersionMismatch && !_isChecking && !Installer.IsRunning; }
		}

		public void InstallLatestVersion()
		{
			Installer.DownloadAndInstall(DayZListingUrl);
		}
	}
}