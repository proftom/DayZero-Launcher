using System;
using System.Collections.ObjectModel;
using System.Linq;
using Dotjosh.DayZCommander.App.Ui;
using Dotjosh.DayZCommander.App.Ui.Friends;

namespace Dotjosh.DayZCommander.App.Core
{
	public class Friend : BindableBase
	{
		private bool _isSelected;

		public string Name { get; set; }

		public Friend(string name)
		{
			Name = name;
			Players = new ObservableCollection<Player>();
		}

		public ObservableCollection<Player> Players { get; set; }

		public bool IsPlaying
		{
			get { return Players.Count > 0; }
		}

		public void NewPlayer(Player newPlayer)
		{
			if(!string.Equals(newPlayer.Name, Name, StringComparison.CurrentCultureIgnoreCase))
			{
				return;
			}

			if(Players.Any(x => x.Hash == newPlayer.Hash))
				return;

			Players.Add(newPlayer);
			PropertyHasChanged("IsPlaying");
			App.Events.Publish(new FriendChanged(this, newPlayer, true));
		}

		public void RemovedPlayer(string oldPlayerHash)
		{
			var player = Players.FirstOrDefault(p => p.Hash == oldPlayerHash);
			if(player == null)
				return;

			Players.Remove(player);
			PropertyHasChanged("IsPlaying");
			App.Events.Publish(new FriendChanged(this, player, false));
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				PropertyHasChanged("IsSelected");
				App.Events.Publish(new FilterByFriendRequest(this, value));
			}
		}
	}
}