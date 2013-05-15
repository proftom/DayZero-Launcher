using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using zombiesnu.DayZeroLauncher.App.Core;
using zombiesnu.DayZeroLauncher.App.Ui.Friends;

namespace zombiesnu.DayZeroLauncher.App.Ui
{
	public class UpdatesViewModel : ViewModelBase, 
		IHandle<ServerUpdated>
	{
		private bool _isVisible;
		//private string STATUS_INPROGRESS = "STATUS_INPROGRESS";
		//private string STATUS_ERROR = "STATUS_ERROR";
		//private string STATUS_DEFAULT = "STATUS_DEFAULT";
		private readonly Dictionary<Server, VersionSnapshot> _processedServers = new Dictionary<Server, VersionSnapshot>();
		private ObservableCollection<VersionStatistic> _rawArma2VersionStats = new ObservableCollection<VersionStatistic>();
		private ObservableCollection<VersionStatistic> _rawDayZVersionStats = new ObservableCollection<VersionStatistic>();
		private int _processedCount;

		public UpdatesViewModel()
		{
			Arma2VersionStats = CollectionViewSource.GetDefaultView(_rawArma2VersionStats) as ListCollectionView;
			Arma2VersionStats.SortDescriptions.Add(new SortDescription("Count", ListSortDirection.Descending));

			DayZVersionStats = CollectionViewSource.GetDefaultView(_rawDayZVersionStats) as ListCollectionView;
			DayZVersionStats.SortDescriptions.Add(new SortDescription("Count", ListSortDirection.Descending));

			LocalMachineInfo = LocalMachineInfo.Current;
			CalculatedGameSettings = CalculatedGameSettings.Current;
			DayZeroLauncherUpdater = new DayZeroLauncherUpdater();
			Arma2Updater = new Arma2Updater();
			DayZUpdater = new DayZUpdater();

			DayZeroLauncherUpdater.PropertyChanged += AnyModelPropertyChanged;
			Arma2Updater.PropertyChanged += AnyModelPropertyChanged;
			DayZUpdater.PropertyChanged += AnyModelPropertyChanged;

			CheckForUpdates();
		}

		private void AnyModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			PropertyHasChanged("Status");
		}

		public string Status
		{
			get
			{
				if(DayZeroLauncherUpdater.Status == DayZeroLauncherUpdater.STATUS_CHECKINGFORUPDATES
					|| Arma2Updater.Status == DayZeroLauncherUpdater.STATUS_CHECKINGFORUPDATES
					|| DayZUpdater.Status == DayZeroLauncherUpdater.STATUS_CHECKINGFORUPDATES)
				return DayZeroLauncherUpdater.STATUS_DOWNLOADING;

				if(DayZeroLauncherUpdater.VersionMismatch 
				   || Arma2Updater.VersionMismatch
				   || DayZUpdater.VersionMismatch)
					return DayZeroLauncherUpdater.STATUS_OUTOFDATE;

				if(!DayZeroLauncherUpdater.VersionMismatch 
					&& !Arma2Updater.VersionMismatch
					&& !DayZUpdater.VersionMismatch)
					return DayZeroLauncherUpdater.STATUS_UPTODATE;

				return "Error";
			}
		}

		public DayZeroLauncherUpdater DayZeroLauncherUpdater { get; private set; }
		public Arma2Updater Arma2Updater { get; private set; }
		public DayZUpdater DayZUpdater { get; private set; }
		public LocalMachineInfo LocalMachineInfo { get; private set; }
		public CalculatedGameSettings CalculatedGameSettings { get; private set; }
		public ListCollectionView Arma2VersionStats { get; private set; }
		public ListCollectionView DayZVersionStats { get; private set; }

		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				PropertyHasChanged("IsVisible");
			}
		}

		public void CheckForUpdates()
		{
		    CalculatedGameSettings.Current.Update();
            LocalMachineInfo.Current.Update();
			DayZeroLauncherUpdater.CheckForUpdate();
			Arma2Updater.CheckForUpdates();
			DayZUpdater.CheckForUpdates();
		}

		public void Handle(ServerUpdated message)
		{
			VersionStatistic existingDayZStatistic = null;
			string dayZVersion = null;
			if(message.Server.DayZVersion != null)
			{
				dayZVersion = message.Server.DayZVersion.ToString();
				existingDayZStatistic = _rawDayZVersionStats.FirstOrDefault(x => x.Version == dayZVersion);
			}

			VersionStatistic existingArma2Statistic = null;
			string arma2Version = null;
			if(message.Server.Arma2Version != null)
			{
				arma2Version = message.Server.Arma2Version.Build.ToString();
				existingArma2Statistic = _rawArma2VersionStats.FirstOrDefault(x => x.Version == arma2Version);
			}

			//If we've seen this server, decrement what it was last time
			var serverWasProcessed = _processedServers.ContainsKey(message.Server);
			if(serverWasProcessed)
			{
				if(existingDayZStatistic != null)
					existingDayZStatistic.Count--;				
				if(existingArma2Statistic != null)
					existingArma2Statistic.Count--;
			}

			if(existingDayZStatistic == null)
			{
				if(message.Server.DayZVersion != null)
					_rawDayZVersionStats.Add(new VersionStatistic() { Version = dayZVersion, Count = 1, Parent = this});
			}
			else
			{
				existingDayZStatistic.Count++;
				_rawDayZVersionStats.Remove(existingDayZStatistic);
				_rawDayZVersionStats.Add(existingDayZStatistic);
			}

			if(existingArma2Statistic == null)
			{
				if( message.Server.Arma2Version != null)
					_rawArma2VersionStats.Add(new VersionStatistic() { Version = arma2Version, Count = 1, Parent = this});
			}
			else
			{
				existingArma2Statistic.Count++;
				_rawArma2VersionStats.Remove(existingArma2Statistic);
				_rawArma2VersionStats.Add(existingArma2Statistic);
			}

			if(!serverWasProcessed)
			{
				_processedServers.Add(message.Server, new VersionSnapshot(message.Server));
				ProcessedCount++;
			}
		}

		public int ProcessedCount
		{
			get { return _processedCount; }
			set
			{
				_processedCount = value;
				PropertyHasChanged("ProcessedCount");
			}
		}
	}

	public class VersionStatistic : BindableBase
	{
		public string Version { get; set; }

		private int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				PropertyHasChanged("Count");
			}
		}

		public UpdatesViewModel Parent { get; set; }
	}

	public class VersionSnapshot
	{
		public VersionSnapshot(Server server)
		{
			if(server.DayZVersion != null)
				DayZVersion = server.DayZVersion.ToString();
			if(server.Arma2Version != null)
				Arma2Version = server.Arma2Version.Build.ToString();
		}

		public string DayZVersion { get; set; }
		public string Arma2Version { get; set; }
	}
}