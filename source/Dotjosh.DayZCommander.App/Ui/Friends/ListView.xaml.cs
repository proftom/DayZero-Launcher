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

namespace Dotjosh.DayZCommander.App.Ui.Friends
{
	/// <summary>
	/// Interaction logic for ListView.xaml
	/// </summary>
	public partial class ListView : UserControl
	{
		public ListView()
		{
			InitializeComponent();
		}

		private void RowDoubleClick(object sender, MouseButtonEventArgs e)
		{			
			var recentServer = (ListViewModel.ServerWithFriends) TheList.SelectedItem;
			if(recentServer == null)
				return;

			GameLauncher.JoinServer(recentServer.Server);
		}
	}
}
