using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Caliburn.Micro;
using Dotjosh.DayZCommander.App.Core;
using Dotjosh.DayZCommander.App.Ui.Friends;

namespace Dotjosh.DayZCommander.App.Ui.ServerList
{
	public class ListViewModel : ViewModelBase,
		IHandle<FilterUpdated>,
		IHandle<ServerUpdated>
	{
		private ListCollectionView _servers;
		private readonly ObservableCollection<Server> _rawServers = new ObservableCollection<Server>();
		private Func<Server, bool> _filter;

		public ListViewModel()
		{
			ReplaceServers();
			Title = "servers";
		}

		private void ReplaceServers()
		{
			Servers = (ListCollectionView) CollectionViewSource.GetDefaultView(_rawServers);
			Servers.SortDescriptions.Add(new SortDescription("Ping", ListSortDirection.Ascending));
			Servers.Filter = Filter;
		}

		private bool Filter(object obj)
		{
			var server = (Server) obj;

			if(_filter != null)
				return _filter(server);
			return true;
		}

		public ObservableCollection<Server> RawServers
		{
			get { return _rawServers; }
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

		public void Handle(FilterUpdated message)
		{
			_filter = message.Filter;
			Servers.Refresh();
		}

		public void Handle(ServerUpdated message)
		{
			if(message.SupressRefresh)
				return; 

			_rawServers.Remove(message.Server);
			_rawServers.Add(message.Server);
		}
	}
}