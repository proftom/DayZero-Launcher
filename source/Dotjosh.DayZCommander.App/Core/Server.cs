using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Dotjosh.DayZCommander.App.Ui.Friends;

namespace Dotjosh.DayZCommander.App.Core
{
	public class Server : BindableBase, IEquatable<Server>
	{
		private readonly string _ipAddress;
		private readonly int _port;
		private readonly ServerQueryClient _queryClient;
		private long _ping;
		private ObservableCollection<Player> _players;
		private SortedDictionary<string, string> _settings;
		public string LastException;
		private bool _isUpdating;

		public Server(string ipAddress, int port)
		{
			_ipAddress = ipAddress;
			_port = port;
			_queryClient = new ServerQueryClient(this, ipAddress, port);
			Settings = new SortedDictionary<string, string>();
			Players = new ObservableCollection<Player>();
			Info = new ServerInfo(null, null);
		}

		public string Id
		{
			get { return _ipAddress + _port; }
		}

		private string _name;
		public string Name
		{
			get
			{
				if(string.IsNullOrEmpty(_name))
				{
					_name = CleanServerName(HostName);
				}
				return _name;
			}
		}

		private string _hostName;
		public string HostName
		{
			get
			{
				if(string.IsNullOrEmpty(_hostName))
				{
					_hostName = GetSettingOrDefault("hostname");
				}
				return _hostName;
			}
		}

		public bool IsFavorite
		{
			get { return UserSettings.Current.IsFavorite(this); }
			set
			{
				if(value)
					UserSettings.Current.AddFavorite(this);
				else
					UserSettings.Current.RemoveFavorite(this);
				PropertyHasChanged("IsFavorite");
			}
		}

		public bool IsSameArmaAndDayZVersion
		{
			get { return IsSameArma2OAVersion && IsSameDayZVersion; }
		}

		public bool IsSameArma2OAVersion
		{
			get
			{
				var reqBuild = GetSettingOrDefault("reqBuild").TryIntNullable();

				return CalculatedGameSettings.Current.Arma2OABetaVersion != null
				       && Arma2Version != null
				       && (reqBuild != null && CalculatedGameSettings.Current.Arma2OABetaVersion.Revision >= reqBuild);
			}
		}

		public bool IsSameDayZVersion
		{
			get
			{
				return CalculatedGameSettings.Current.DayZVersion != null
				       && DayZVersion != null
				       && CalculatedGameSettings.Current.DayZVersion.Equals(DayZVersion);
			}
		}

		public void NotifyGameVersionChanged()
		{
			PropertyHasChanged("IsSameArma2OAVersion", "IsSameDayZVersion", "IsSameArmaAndDayZVersion");
		}

		public int? CurrentPlayers
		{
			get { return GetSettingOrDefault("numplayers").TryIntNullable(); }
		}

		public int? MaxPlayers
		{
			get { return GetSettingOrDefault("maxplayers").TryIntNullable(); }
		}

        public static Regex ServerTimeRegex = new Regex(@"((GmT|Utc)[\s]*(?<Offset>([+]|[-])[\s]?[\d]{1,2})?)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private ServerInfo _info;

		public DateTime? ServerTime
		{
			get
			{
				var name = GetSettingOrDefault("hostname");
				if(string.IsNullOrWhiteSpace(name))
					return null;

				var match = ServerTimeRegex.Match(name);
				if(!match.Success)
					return null;

				var offset = match.Groups["Offset"].Value.Replace(" ", "");
                if (offset == "") offset = "0";
				var offsetInt = int.Parse(offset);

				return DateTime.UtcNow
							.AddHours(offsetInt);
			}
		}

		public SortedDictionary<string, string> Settings
		{
			get { return _settings; }
			internal set
			{
				_settings = value;
				Info = new ServerInfo((ServerDifficulty?) Difficulty, Name);
				PropertyHasChanged("Settings");
				PropertyHasChanged("Name");
				PropertyHasChanged("CurrentPlayers");
				PropertyHasChanged("MaxPlayers");
				PropertyHasChanged("ServerTime");
				PropertyHasChanged("HasPassword");
				PropertyHasChanged("Difficulty");
				NotifyGameVersionChanged();
			}
		}

		public ServerInfo Info
		{
			get { return _info; }
			private set
			{
				_info = value;
				PropertyHasChanged("Info");
			}
		}

		public bool IsUpdating
		{
			get { return _isUpdating; }
			private set
			{
				_isUpdating = value;
				Execute.OnUiThread(() => PropertyHasChanged("IsUpdating"));
			}
		}

		public long Ping
		{
			get
			{
				if(LastException != null)
				{
					return 10 * 1000;
				}
				return _ping;
			}
			set
			{
				_ping = value;
				PropertyHasChanged("Ping");
			}
		}

		public ObservableCollection<Player> Players
		{
			get { return _players; }
			private set
			{
				_players = value;
				PropertyHasChanged("Players");
			}
		}

		public string IpAddress
		{
			get { return _ipAddress; }
		}

		public int? Difficulty
		{
			get { return GetSettingOrDefault("difficulty").TryIntNullable(); }
		}

		public int FreeSlots
		{
			get 
			{ 
				if(MaxPlayers != null && CurrentPlayers != null)
				{
					return (int) (MaxPlayers - CurrentPlayers);
				}
				return 0;
			}
		}

		public bool IsEmpty
		{
			get { return CurrentPlayers == null || CurrentPlayers == 0; }
		}

		public int Port
		{
			get { return _port; }
		}

		public string LastJoinedOn
		{
			get
			{
				var recent = UserSettings.Current.RecentServers
								.OrderByDescending(x => x.On)
								.FirstOrDefault(x => x.Server == this);
				if(recent == null)
					return "Never";

				return recent.Ago;
			}
		}

		public string Notes
		{
			get
			{
				return UserSettings.Current.GetNotes(this);
			}
			set
			{
				UserSettings.Current.SetNotes(this, value);
				_hasNotes = !string.IsNullOrEmpty(value);
				PropertyHasChanged("Notes", "HasNotes");
			}
		}

		private bool? _hasNotes;
		public bool HasNotes
		{
			get
			{
				if(_hasNotes != null)
					return (bool) _hasNotes;
				_hasNotes = UserSettings.Current.HasNotes(this);
				return (bool) _hasNotes;
			}
		}

		public bool? IsNight
		{
			get
			{
				var serverTime = ServerTime;
				if(serverTime == null)
					return null;

				return serverTime.Value.Hour < 5 || serverTime.Value.Hour > 19;
			}
		}

		public bool HasPassword
		{
			get { return GetSettingOrDefault("password").TryInt() > 0; }
		}

		public bool Hive
		{
			get { return GetSettingOrDefault("mod").SafeContainsIgnoreCase("@hive"); }
		}

		private Version _arma2Version;
		public Version Arma2Version
		{
			get
			{
				if(_arma2Version == null)
				{
					var arma2VersionString = GetSettingOrDefault("gamever");
					Version.TryParse(arma2VersionString, out _arma2Version);
				}
				return _arma2Version;
			}
		}

		private Version _dayZVersion;
		public Version DayZVersion
		{
			get
			{
				if(_dayZVersion == null)
				{
					var dayZVersionString = GetDayZVersionString(Name);
					Version.TryParse(dayZVersionString, out _dayZVersion);
				}
				return _dayZVersion;
			}
		}

		public bool ProtectionEnabled
		{
			get
			{
				return GetSettingOrDefault("verifySignatures").TryInt() > 0
				       && GetSettingOrDefault("sv_battleye").TryInt() > 0;
			}
		}

		private string GetSettingOrDefault(string settingName)
		{
			if (Settings.ContainsKey(settingName))
			{
				return Settings[settingName];
			}
			return null;
		}

		public void Update(bool supressRefresh=false)
		{
			try
			{
				IsUpdating = true;
				var serverResult = _queryClient.Execute();
				Execute.OnUiThread(() =>
				                    	{
											App.Events.Publish(new PlayersChangedEvent(Players, serverResult.Players));
				                    		Players = new ObservableCollection<Player>(serverResult.Players.OrderBy(x => x.Name));
				                    		LastException = null;
				                    		Settings = serverResult.Settings;
				                    		Ping = serverResult.Ping;
											App.Events.Publish(new ServerUpdated(this, supressRefresh));
				                    	});
			}
			catch (Exception ex)
			{
				Execute.OnUiThread(() =>
				                    	{
											LastException = ex.Message;
											PropertyHasChanged("Name", "Ping");
											App.Events.Publish(new ServerUpdated(this, supressRefresh));
				                    	});
				
			}
			finally
			{
				IsUpdating = false;
			}
		}

		private static string CleanServerName(string name)
		{
			if(string.IsNullOrEmpty(name))
			{
				return name;
			}

			var cleanName = name.Trim();

			cleanName = Regex.Replace(cleanName, @"^DayZ\s*(Zombie){0,1}\s*(RPG){0,1}\s*-\s*", "", RegexOptions.IgnoreCase);

			return cleanName.Trim();
		}

		private static string GetDayZVersionString(string name)
		{
			if(string.IsNullOrEmpty(name))
			{
				return null;
			}

			var match = Regex.Match(name, @"\d(\.\d){1,3}");
			if(!match.Success)
			{
				return null;
			}
			return match.Value;
		}

		public bool Equals(Server other)
		{
			if(other == null)
				return false;
			return (other.IpAddress == this.IpAddress
			        && other.Port == this.Port);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public void BeginUpdate(Action<Server> onComplete, bool supressRefresh=false)
		{
			new Thread(() =>
			{
				try
				{
					Update(supressRefresh);
				}
				finally
				{
					onComplete(this);
				}
			}, 1).Start();			
		}
	}
}