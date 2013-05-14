using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using Dotjosh.DayZCommander.App.Core;
using Dotjosh.DayZCommander.App.Ui.Friends;

namespace Dotjosh.DayZCommander.App.Ui.Favorites
{
	public class FavoritesViewModel : ViewModelBase, 
		IHandle<FavoritesUpdated>,
		IHandle<ServerUpdated>
	{
		private ObservableCollection<Server> _rawServers = new ObservableCollection<Server>();

		public FavoritesViewModel()
		{
			Title = "favorites";

			Servers = (ListCollectionView) CollectionViewSource.GetDefaultView(_rawServers);
			Servers.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

			foreach(var favorite in UserSettings.Current.Favorites)
			{
				var server = favorite.CreateServer();
//				server.BeginUpdate(s => { });
				App.Events.Publish(new FavoritesUpdated(server));
			}
		}

		public ListCollectionView Servers { get; private set; }

		public ObservableCollection<Server> RawServers
		{
			get { return _rawServers; }
		}

		public void Handle(FavoritesUpdated message)
		{
			if(UserSettings.Current.IsFavorite(message.Server))
			{
				if(_rawServers.Contains(message.Server))
				{
					_rawServers.Remove(message.Server);
				}
				_rawServers.Add(message.Server);
			}
			else
			{
				if(_rawServers.Contains(message.Server))
					_rawServers.Remove(message.Server);
			}
			SyncTitle();
		}

		private void SyncTitle()
		{
			var count = _rawServers.Count;
			if(count == 0)
				Title = "favorites";
			else
				Title = string.Format("favorites({0})", count);
		}

		public void Handle(ServerUpdated message)
		{
			var matchingServer = _rawServers.FirstOrDefault(s => s.Equals(message.Server));
			if(matchingServer != null)
			{
				_rawServers.Remove(matchingServer);
				_rawServers.Add(message.Server);
			}
		}
	}
}
