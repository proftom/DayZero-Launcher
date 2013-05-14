using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui.Friends
{
	public class ServerUpdated
	{
		public Server Server { get; set; }
		public bool SupressRefresh { get; set; }

		public ServerUpdated(Server server, bool supressRefresh)
		{
			Server = server;
			SupressRefresh = supressRefresh;
		}
	}
}