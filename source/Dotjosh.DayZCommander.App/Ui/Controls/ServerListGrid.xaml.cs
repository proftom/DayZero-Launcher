using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Caliburn.Micro;
using zombiesnu.DayZeroLauncher.App.Core;

namespace zombiesnu.DayZeroLauncher.App.Ui.Controls
{
	/// <summary>
	/// Interaction logic for ServerListGrid.xaml
	/// </summary>
	public partial class ServerListGrid : UserControl,
		IHandle<RefreshingServersChange>
	{

		public ServerListGrid()
		{
			InitializeComponent();
			App.Events.Subscribe(this);
		}

		private void RowDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var control = e.OriginalSource as FrameworkElement;
			if(control != null)
			{
				if(control.Name == "Refresh")
				{
					e.Handled = true;
					return;
				}
			}
			var server = (Server) ((Control) sender).DataContext;

			GameLauncher.JoinServer(server);
		}

        void RowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                e.Handled = true;
        }

        void RowKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                GameLauncher.JoinServer((Server)((Control)sender).DataContext);
        }

		private void RowLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			App.Events.Publish(new DataGridRowSelected());
		}

		private void RefreshAllServer(object sender, RoutedEventArgs e)
		{
			var servers = ((IEnumerable) TheGrid.DataContext)
								.Cast<Server>()
								.ToList();
			var batch = new ServerBatchRefresher("Refreshing some servers...", servers);
			App.Events.Publish(new RefreshServerRequest(batch));
		}

		private void RefreshAllServersDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		public void Handle(RefreshingServersChange message)
		{
			var column = TheGrid.Columns[5];
			var originalStyle = column.HeaderStyle;
			var newStyle = new Style(typeof (DataGridColumnHeader), originalStyle);
				newStyle.Setters.Add(new Setter(VisibilityProperty, message.IsRunning ? Visibility.Hidden : Visibility.Visible));
			column.HeaderStyle = newStyle;
		}
	}

	public class RefreshServerRequest
	{
		public ServerBatchRefresher Batch { get; set; }

		public RefreshServerRequest(ServerBatchRefresher batch)
		{
			Batch = batch;
		}
	}

	public class DataGridRowSelected
	{
	}
}
