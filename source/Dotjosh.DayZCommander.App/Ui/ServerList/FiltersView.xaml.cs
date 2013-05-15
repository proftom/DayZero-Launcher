using System.Windows.Controls;
using System.Windows.Input;

namespace zombiesnu.DayZeroLauncher.App.Ui.ServerList
{
	/// <summary>
	/// Interaction logic for FiltersView.xaml
	/// </summary>
	public partial class FiltersView : UserControl
	{
		public FiltersView()
		{
			InitializeComponent();

		}

		private void Name_KeyUp(object sender, KeyEventArgs e)
		{
			NameEntry.GetBindingExpression(TextBox.TextProperty).UpdateSource();
		}
	}
}
