using System;
using zombiesnu.DayZeroLauncher.App.Core;

namespace zombiesnu.DayZeroLauncher.App.Ui.ServerList
{
	public class FilterUpdated
	{
		public Func<Server, bool> Filter { get; set; }

		public FilterUpdated(Func<Server, bool> filter)
		{
			Filter = filter;
		}
	}
}