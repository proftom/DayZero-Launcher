using System.Collections.Generic;
using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui.Friends
{
	public class PlayersChangedEvent
	{
		public IEnumerable<Player> OldPlayers { get; set; }
		public IEnumerable<Player> NewPlayers { get; set; }

		public PlayersChangedEvent(IEnumerable<Player> oldPlayers, IEnumerable<Player> newPlayers)
		{
			OldPlayers = oldPlayers;
			NewPlayers = newPlayers;
		}
	}
}