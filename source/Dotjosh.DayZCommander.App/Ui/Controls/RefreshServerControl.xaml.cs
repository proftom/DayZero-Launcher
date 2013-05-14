using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui.Controls
{
	/// <summary>
	/// Interaction logic for RefreshServerControl.xaml
	/// </summary>
	public partial class RefreshServerControl : UserControl
	{
		public RefreshServerControl()
		{
			InitializeComponent();
		}

		private void RefreshServer(object sender, RoutedEventArgs e)
		{
			var server = (Server) ((Control) sender).DataContext;
			server.BeginUpdate(server1 => {}, supressRefresh:true);
		}

		private void RefreshServerDoubleClick(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
	}
}
