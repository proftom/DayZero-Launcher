using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using zombiesnu.DayZeroLauncher.App.Core;
using zombiesnu.DayZeroLauncher.App.Ui.Controls;
using zombiesnu.DayZeroLauncher.App.Ui.Friends;

namespace zombiesnu.DayZeroLauncher.App.Ui
{
	public class NotesViewModel : ViewModelBase, 
		IHandle<RequestNoteEdit>
	{
		private bool _isVisible;
		private Server _server;

		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				PropertyHasChanged("IsVisible");
			}
		}

		public Server Server
		{
			get { return _server; }
			set
			{
				_server = value;
				PropertyHasChanged("Server");
			}
		}

		public void Handle(RequestNoteEdit message)
		{
			Server = message.Server;
			IsVisible = true;
		}
	}
}