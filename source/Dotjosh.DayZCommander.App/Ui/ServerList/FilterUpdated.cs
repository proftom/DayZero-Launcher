using System;
using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui.ServerList
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