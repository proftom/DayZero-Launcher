using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Caliburn.Micro;
using Dotjosh.DayZCommander.App.Core;
using Dotjosh.DayZCommander.App.Ui.Friends;

namespace Dotjosh.DayZCommander.App.Ui.Controls
{
	/// <summary>
	/// Interaction logic for ServerStandaloneHeaderRow.xaml
	/// </summary>
	public partial class ServerStandaloneHeaderRow : UserControl,
		IHandle<RefreshingServersChange>
	{
		public ServerStandaloneHeaderRow()
		{
			InitializeComponent();
			App.Events.Subscribe(this);
		}

		private void RefreshAllServer(object sender, RoutedEventArgs e)
		{
			var friends = DataContext as Friends.ListViewModel;
			if(friends != null)
			{
				var batch = new ServerBatchRefresher("Refreshing servers with friends...", friends.Servers.Cast<ListViewModel.ServerWithFriends>().Select(swf => swf.Server).ToList());
				App.Events.Publish(new RefreshServerRequest(batch));
				return;
			}

			var recent = DataContext as Recent.RecentViewModel;
			if(recent != null)
			{
				var batch = new ServerBatchRefresher("Refreshing recent servers...", recent.Servers.Select(r => r.Server).ToList());
				App.Events.Publish(new RefreshServerRequest(batch));
			}

		}

		private void RefreshAllServersDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		public void Handle(RefreshingServersChange message)
		{
			RefreshAllButton.Visibility = message.IsRunning
			                              	? Visibility.Hidden
			                              	: Visibility.Visible;
		}
	}
}
