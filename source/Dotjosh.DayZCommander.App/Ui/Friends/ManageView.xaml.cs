using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using zombiesnu.DayZeroLauncher.App.Core;

namespace zombiesnu.DayZeroLauncher.App.Ui.Friends
{
	/// <summary>
	/// Interaction logic for FriendsList.xaml
	/// </summary>
	public partial class ManageView : UserControl
	{
		public ManageView()
		{
			InitializeComponent();
		}

		private ManageViewModel ViewModel
		{
			get { return (ManageViewModel) DataContext; }
		}

		private void NewFriend(object sender, RoutedEventArgs e)
		{
			ViewModel.NewFriend();
			NewFriendName.Focus();
		}

		private void NewFriendName_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				NewFriendName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
				ViewModel.CreateFriend();
			}
		}

		private void NewFriendName_LostFocus(object sender, RoutedEventArgs e)
		{
		//	ViewModel.CancelNewFriend();
		}

		private void DeleteFriend(object sender, RoutedEventArgs e)
		{
			var friend = (Friend) ((Control) sender).DataContext;
			ViewModel.DeleteFriend(friend);
		}
	}
}
