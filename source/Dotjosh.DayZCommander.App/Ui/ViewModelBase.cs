using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui
{
	public abstract class ViewModelBase : BindableBase
	{
		private string _title;
		private bool _isSelected;

		protected ViewModelBase()
		{
			App.Events.Subscribe(this);
		}

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				PropertyHasChanged("Title");
			}
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				PropertyHasChanged("IsSelected");
			}
		}
	}
}