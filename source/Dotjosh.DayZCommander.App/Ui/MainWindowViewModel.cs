using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using zombiesnu.DayZeroLauncher.App.Core;
using zombiesnu.DayZeroLauncher.App.Ui.Favorites;
using zombiesnu.DayZeroLauncher.App.Ui.Friends;
using zombiesnu.DayZeroLauncher.App.Ui.Recent;
using zombiesnu.DayZeroLauncher.App.Ui.ServerList;

namespace zombiesnu.DayZeroLauncher.App.Ui
{
	public class MainWindowViewModel : ViewModelBase, 
		IHandle<RepublishFriendsRequest>
	{
		private Core.ServerList _serverList;
		private ViewModelBase _currentTab;
		private ObservableCollection<ViewModelBase> _tabs;

		public MainWindowViewModel()
		{
			Tabs = new ObservableCollection<ViewModelBase>(new ViewModelBase[]
			                                               	{
			                                               		ServerListViewModel = new ServerListViewModel()
			                                               	});
			CurrentTab = Tabs.First();

			ServerList = new Core.ServerList();

			ServerList.GetAndUpdateAll();

			SettingsViewModel = new	SettingsViewModel();
			UpdatesViewModel = new UpdatesViewModel();
		}

		public DayZeroLauncherUpdater Updater { get; private set; }
		public ServerListViewModel ServerListViewModel { get; set; }
		public SettingsViewModel SettingsViewModel { get; set; }
		public UpdatesViewModel UpdatesViewModel { get; set; }


		public Core.ServerList ServerList
		{
			get { return _serverList; }
			set
			{
				_serverList = value;
				PropertyHasChanged("ServerList");
			}
		}

		public bool IsServerListSelected
		{
			get { return CurrentTab == ServerListViewModel; }
		}

	
		public ViewModelBase CurrentTab
		{
			get { return _currentTab; }
			set
			{
				if(_currentTab != null)
					_currentTab.IsSelected = false;
				_currentTab = value;
				if(_currentTab != null)
					_currentTab.IsSelected = true;
				PropertyHasChanged("CurrentTab", "IsServerListSelected");
			}
		}

		public ObservableCollection<ViewModelBase> Tabs
		{
			get { return _tabs; }
			set
			{
				_tabs = value;
				PropertyHasChanged("Tabs");
			}
		}

		public void Handle(RepublishFriendsRequest message)
		{
			foreach(var server in ServerList.Items)
			{
				App.Events.Publish(new PlayersChangedEvent(server.Players, server.Players));
			}
		}

		public void ShowSettings()
		{
			SettingsViewModel.IsVisible = true;
			UpdatesViewModel.IsVisible = false;
		}

		public void ShowUpdates()
		{
			SettingsViewModel.IsVisible = false;
			UpdatesViewModel.IsVisible = true;
		}

		public void Escape()
		{
			SettingsViewModel.IsVisible = false;
			UpdatesViewModel.IsVisible = false;
		}
	}
}