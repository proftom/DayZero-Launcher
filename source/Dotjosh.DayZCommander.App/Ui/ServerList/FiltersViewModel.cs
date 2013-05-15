using System;
using System.Collections.Generic;
using Caliburn.Micro;
using zombiesnu.DayZeroLauncher.App.Core;

namespace zombiesnu.DayZeroLauncher.App.Ui.ServerList
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