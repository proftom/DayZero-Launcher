using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using Dotjosh.DayZCommander.App.Ui.Friends;
using Dotjosh.DayZCommander.App.Ui.Recent;

namespace Dotjosh.DayZCommander.App.Core
{
	[DataContract]
	public class UserSettings
	{
		private static UserSettings _current;

		[DataMember] private List<string> _friends = new List<string>();
		[DataMember] private Filter _filter = new Filter();
		[DataMember] private WindowSettings _windowSettings = null; //This is null on purpose so the MainWindow view can set defaults if needed
		[DataMember] private GameOptions _gameOptions = new GameOptions();
		[DataMember] private AppOptions _appOptions = new AppOptions();
		[DataMember] private List<FavoriteServer> _favorites = new List<FavoriteServer>();
		[DataMember] private List<RecentServer> _recentServers = new List<RecentServer>();

		public List<string> Friends
		{
			get
			{
				if(_friends == null)
				{
					_friends = new List<string>();
				}
				return _friends;
			}
			set { _friends = value; }
		}

		public Filter Filter
		{
			get
			{
				if(_filter == null)
				{
					_filter = new Filter();
				}
				return _filter;
			}
			set { _filter = value; }
		}

		public WindowSettings WindowSettings
		{
			get { return _windowSettings; }
			set { _windowSettings = value; }
		}

		public GameOptions GameOptions
		{
			get
			{
				if(_gameOptions == null)
				{
					_gameOptions = new GameOptions();
				}
				return _gameOptions;
			}
			set { _gameOptions = value; }
		}

		public AppOptions AppOptions
		{
			get
			{
				if(_appOptions == null)
				{
					_appOptions = new AppOptions();
				}
				return _appOptions;
			}
			set { _appOptions = value; }
		}

		public List<FavoriteServer> Favorites
		{
			get
			{
				if(_favorites == null)
				{
					_favorites = new List<FavoriteServer>();
				}
				return _favorites;
			}
			set { _favorites = value; }
		}

		public List<RecentServer> RecentServers
		{
			get
			{
				if(_recentServers == null)
				{
					_recentServers = new List<RecentServer>();
				}
				return _recentServers;
			}
			set { _recentServers = value; }
		}

		public void Save()
		{
			try
			{
				lock(_fileLock)
				{
					using (var fs = GetSettingsFileStream(FileMode.Create))
					{
						var serializer = new DataContractSerializer(GetType());
						serializer.WriteObject(fs, this);
						fs.Flush(true);
					}
				}
			}
			catch(Exception)
			{
				
			}
		}

		private static UserSettings Load()
		{
			try
			{
				using(var fs = GetSettingsFileStream(FileMode.Open))
				{
					using(var reader = new StreamReader(fs))
					{
						var rawXml = reader.ReadToEnd();
						if(string.IsNullOrWhiteSpace(rawXml))
						{
							return new UserSettings();
						}
						else
						{
							return LoadFromXml(XDocument.Parse(rawXml));
						}
					}
				}
			}
			catch(FileNotFoundException)
			{
				return new UserSettings();
			}
		}

		private static FileStream GetSettingsFileStream(FileMode fileMode)
		{
			return new FileStream(SettingsPath, fileMode);		
		}

		private static object _fileLock = new object();
		public static UserSettings Current
		{
			get
			{
				lock(_fileLock)
				{
					if (_current == null)
					{
						try
						{
							_current = Load();
						}
						catch (Exception ex)
						{
							_current = new UserSettings();
						}
					}
				}
				return _current;
			}
		}

		private static string _settingsPath;
		private static string SettingsPath
		{
			get
			{
				if(_settingsPath == null)
				{

					var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
					var dayzCommanderAppDataDirectory = new DirectoryInfo(Path.Combine(appDataFolder, "DayZCommander"));
					if (!dayzCommanderAppDataDirectory.Exists)
						dayzCommanderAppDataDirectory.Create();
					var newFileLocation = Path.Combine(dayzCommanderAppDataDirectory.FullName, "settings.xml");

					//Migrate old settings location
					try
					{
						var oldAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
						var oldFileLocation = Path.Combine(oldAppDataFolder, "DayZCommander", "settings.xml");
						;
						if (File.Exists(oldFileLocation) && !File.Exists(newFileLocation))
						{
							File.Move(oldFileLocation, newFileLocation);
							Directory.Delete(new FileInfo(oldFileLocation).Directory.FullName);
						}
					}
					catch (Exception ex)
					{
					}
					_settingsPath = newFileLocation;
				}
				return _settingsPath;
			}
		}

		private static UserSettings LoadFromXml(XDocument xDocument)
		{
			var serializer = new DataContractSerializer(typeof(UserSettings));
			return (UserSettings)serializer.ReadObject(xDocument.CreateReader());
		}

		public bool IsFavorite(Server server)
		{
			return Favorites.Any(f => f.Matches(server));
		}

		public void AddFavorite(Server server)
		{
			if(Favorites.Any(f => f.Matches(server)))
				return;
			Favorites.Add(new FavoriteServer(server));
			App.Events.Publish(new FavoritesUpdated(server));
			Save();
		}

		public void RemoveFavorite(Server server)
		{
			var favorite = Favorites.FirstOrDefault(f => f.Matches(server));
			if(favorite == null)
				return;
			Favorites.Remove(favorite);
			App.Events.Publish(new FavoritesUpdated(server));
			Save();
		}

		public void AddRecent(Server server)
		{
			var recentServer = new RecentServer(server, DateTime.Now);
			if(RecentServers.Count > 50)
			{
				var oldest = RecentServers.OrderBy(x => x.On).FirstOrDefault();
				RecentServers.Remove(oldest);
			}
			RecentServers.Add(recentServer);
			recentServer.Server = server;
			App.Events.Publish(new RecentAdded(recentServer));
			Save();			
		}

		public string GetNotes(Server server)
		{
			var fileName = GetNoteFileName(server);
			if(!File.Exists(fileName))
				return "";
			return File.ReadAllText(fileName, Encoding.UTF8);
		}

		public void SetNotes(Server server, string text)
		{
			var fileName = GetNoteFileName(server);
			if(string.IsNullOrWhiteSpace(text))
			{
				if(File.Exists(fileName))
					File.Delete(fileName);
			}
			else
			{
				File.WriteAllText(fileName, text, Encoding.UTF8);
			}
		}

		private static string GetNoteFileName(Server server)
		{
			return Path.Combine(new FileInfo(SettingsPath).Directory.FullName, string.Format("Notes_{0}_{1}.txt", server.IpAddress.Replace(".", "_"), server.Port));
		}

		public bool HasNotes(Server server)
		{
			return File.Exists(GetNoteFileName(server));
		}
	}

	public class RecentAdded
	{
		public RecentServer Recent { get; set; }

		public RecentAdded(RecentServer recent)
		{
			Recent = recent;
		}
	}

	public class FavoritesUpdated
	{
		public Server Server { get; set; }

		public FavoritesUpdated(Server server)
		{
			Server = server;
		}
	}
}