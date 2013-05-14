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
	/// Interaction logic for NotesControl.xaml
	/// </summary>
	public partial class NotesControl : UserControl
	{
		public NotesControl()
		{
			InitializeComponent();
		}

		private void Note_Click(object sender, RoutedEventArgs e)
		{
			var server = (Server) ((Control) sender).DataContext;
			App.Events.Publish(new RequestNoteEdit(server));
		}
	}

	public class RequestNoteEdit
	{
		public Server Server { get; set; }

		public RequestNoteEdit(Server server)
		{
			Server = server;
		}
	}
}
