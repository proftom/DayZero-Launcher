using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using zombiesnu.DayZeroLauncher.App.Core;

namespace zombiesnu.DayZeroLauncher.App.Ui.Friends
{
	public class ListViewModel : ViewModelBase,
		IHandle<FriendChanged>
	{
		private ObservableCollection<ServerWithFriends> _rawServersWithFriends;
		private ListCollectionView _servers;

		public ListViewModel()
		{
			_rawServersWithFriends = new ObservableCollection<ServerWithFriends>();
			Servers = (ListCollectionView) CollectionViewSource.GetDefaultView(_rawServersWithFriends);
			Servers.SortDescriptions.Add(new SortDescription("Friends.Count", ListSortDirection.Descending));
			Servers.Refresh();
		}

		#region Implementation of IHandle<FriendsChanged>

		public void Handle(FriendChanged message)
		{
			if(message.IsAdded)
			{
				var existingServer = _rawServersWithFriends.FirstOrDefault(s => s.Server == message.Player.Server);
				if(existingServer == null)
				{
					existingServer = new ServerWithFriends()
					{
						Server = message.Player.Server,
						Friends = new ObservableCollection<Player>(new [] { message.Player })
					};
					_rawServersWithFriends.Add(existingServer);
				}
				else
				{
					if(!existingServer.Friends.Any(f => string.Equals(f.Name, message.Friend.Name, StringComparison.CurrentCultureIgnoreCase)))
					{
						existingServer.Friends.Add(message.Player);
					}
				}
			}
			else
			{
				var serverWithThisPlayer = _rawServersWithFriends
										.FirstOrDefault(x => x.Friends.Any(f => f == message.Player));
				if(serverWithThisPlayer != null)
				{
					serverWithThisPlayer.Friends.Remove(message.Player);
					if(serverWithThisPlayer.Friends.Count == 0)
					{
						_rawServersWithFriends.Remove(serverWithThisPlayer);
					}
				}
			}
			Servers.Refresh();
		}

		public class ServerWithFriends
		{
			public Server Server { get; set; }
			public ObservableCollection<Player> Friends { get; set; }
		}

		public ListCollectionView Servers
		{
			get { return _servers; }
			set
			{
				_servers = value;
				PropertyHasChanged("Servers");
			}
		}

		#endregion
	}
}