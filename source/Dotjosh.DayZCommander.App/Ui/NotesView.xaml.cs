using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui
{
	/// <summary>
	/// Interaction logic for NotesView.xaml
	/// </summary>
	public partial class NotesView : UserControl
	{
		public NotesView()
		{
			InitializeComponent();

			KeyUp += OnKeyUp;
			KeyDown += OnKeyDown;

			DataContextChanged += OnDataContextChanged;
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			if(args.NewValue != null)
			{
				((INotifyPropertyChanged)args.NewValue).PropertyChanged += OnPropertyChanged;
			}
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if(args.PropertyName == "IsVisible")
			{
				if(ViewModel.IsVisible)
				{
					Dispatcher.BeginInvoke(
							DispatcherPriority.Input, 
							new ThreadStart(() => NotesEntry.Focus()));
				}
			}
		}

		protected NotesViewModel ViewModel
		{
			get { return (NotesViewModel)DataContext; }
		}

		private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
		{
			if(keyEventArgs.Key == Key.Enter && Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				NotesEntry.GetBindingExpression(TextBox.TextProperty).UpdateSource();
				((NotesViewModel) (((Control) sender).DataContext)).IsVisible = false;
			}
		}

		private void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
		{
			if(keyEventArgs.Key == Key.Escape)
			{
				NotesEntry.GetBindingExpression(TextBox.TextProperty).UpdateSource();
			}
		}



		private void Done_Click(object sender, RoutedEventArgs e)
		{
			((NotesViewModel) (((Control) sender).DataContext)).IsVisible = false;
		}
	}
}
