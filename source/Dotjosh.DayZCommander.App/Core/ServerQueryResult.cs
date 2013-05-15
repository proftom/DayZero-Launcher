using System.Collections.Generic;

namespace zombiesnu.DayZeroLauncher.App.Core
{
	public class ServerQueryResult
	{
		public long Ping { get; set; }
		public SortedDictionary<string, string> Settings { get; set; }
		public List<Player> Players { get; set; }
	}
}