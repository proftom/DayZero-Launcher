using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using Caliburn.Micro;
using Dotjosh.DayZCommander.App.Core;
using Dotjosh.DayZCommander.App.Ui.Friends;

namespace Dotjosh.DayZCommander.App.Ui.Recent
{
	public class RecentViewModel : ViewModelBase, 
		IHandle<RecentAdded>,
		IHandle<ServerUpdated>
	{
		private ObservableCollection<RecentServer> _servers;
		private Dictionary<string, List<RecentServer>> _serverDictionary = new Dictionary<string, List<RecentServer>>();
		private Timer _updateTimeTimer;

		public RecentViewModel()
		{
			Title = "recent";

			Servers = new ObservableCollection<RecentServer>();

			foreach(var recent in UserSettings.Current.RecentServers)
			{
				recent.CreateServer();
				AddToDictionary(recent);
			}

			_updateTimeTimer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
			_updateTimeTimer.Elapsed += UpdateTimeTimerOnElapsed;
			_updateTimeTimer.Start();

			UpdateServersByDateViewModel();
		}

		private void UpdateTimeTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			_updateTimeTimer.Stop();
			Execute.OnUiThread(() =>
			                   	{
			                   		for(var i = UserSettings.Current.RecentServers.Count -1; i >= 0; i--)
			                   		{
			                   			UserSettings.Current.RecentServers[i].RefreshAgo();
			                   		}
			                   	});

			_updateTimeTimer.Start();
		}

		public ObservableCollection<RecentServer> Servers
		{
			get { return _servers; }
			set
			{
				_servers = value;
				PropertyHasChanged("Servers");
			}
		}

		public void Handle(RecentAdded message)
		{
			AddToDictionary(message.Recent);
			UpdateServersByDateViewModel();
		}

		private void AddToDictionary(RecentServer recent)
		{
			var key = GetKey(recent.Server);
			if (!_serverDictionary.ContainsKey(key))
			{
				_serverDictionary.Add(key, new List<RecentServer>());
			}
			_serverDictionary[key].Add(recent);
		}

		private void UpdateServersByDateViewModel()
		{
			Servers.Clear();
			var servers = UserSettings.Current.RecentServers
							.OrderByDescending(s => s.On);

			Servers = new ObservableCollection<RecentServer>(servers);
		}

		public void Handle(ServerUpdated message)
		{
			var key = GetKey(message.Server);
			if(_serverDictionary.ContainsKey(key))
			{
				foreach(var recent in _serverDictionary[key])
				{
					recent.Server = message.Server;
				}
			}
		}

		private string GetKey(Server server)
		{
			return server.IpAddress + server.Port;
		}
	}
}
	