using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui.ServerList
{
	public class FiltersViewModel : ViewModelBase
	{
		public FiltersViewModel()
		{
			Title = "filters";

			Filter = UserSettings.Current.Filter;
		}

		public Filter Filter { get; set; }
	}
}