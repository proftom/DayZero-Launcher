using System;
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
using zombiesnu.DayZeroLauncher.App.Core;

namespace zombiesnu.DayZeroLauncher.App.Ui.Controls
{
	/// <summary>
	/// Interaction logic for FavoriteControl.xaml
	/// </summary>
	public partial class FavoriteControl : UserControl
	{
		public FavoriteControl()
		{
			InitializeComponent();
		}


		private void IsFavorite_Checked(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			var button = (ToggleButton) sender;
			var server = (Server) button.DataContext;

			server.IsFavorite = (bool) button.IsChecked;
		}

		private void IsFavorite_UnChecked(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
			var button = (ToggleButton) sender;
			var server = (Server) button.DataContext;

			server.IsFavorite = (bool) button.IsChecked;
		}
	}
}
