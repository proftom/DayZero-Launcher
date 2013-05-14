using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Caliburn.Micro;
using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui.Friends
{
	public class ManageViewModel : ViewModelBase,
	                                    IHandle<PlayersChangedEvent>
	{
		private bool _isAdding;
		private string _newFriendName;

		public ManageViewModel()
		{
			Friends = new ObservableCollection<Friend>();

			foreach (string friendName in UserSettings.Current.Friends)
			{
				Friends.Add(new Friend(friendName));
			}
			Title = "friends";
		}

		public bool IsAdding
		{
			get { return _isAdding; }
			set
			{
				_isAdding = value;
				PropertyHasChanged("IsAdding");
			}
		}

		public string NewFriendName
		{
			get { return _newFriendName; }
			set
			{
				_newFriendName = value;
				PropertyHasChanged("NewFriendName");
			}
		}

		private ObservableCollection<Friend> _friends;
		public ObservableCollection<Friend> Friends
		{
			get { return _friends; }
			private set
			{
				_friends = value;
				PropertyHasChanged("Friends");
			}
		}

		#region IHandle<PlayersChangedEvent> Members

		public void Handle(PlayersChangedEvent message)
		{
			foreach (Player oldPlayer in message.OldPlayers)
			{
				string oldPlayerHash = oldPlayer.Hash;
				bool wasRemoved = message
					.NewPlayers
					.None(newPlayer => newPlayer.Hash == oldPlayerHash);

				if (wasRemoved)
					Remove(oldPlayerHash);
			}

			foreach (Player newPlayer in message.NewPlayers)
			{
				Add(newPlayer);
			}
		}

		#endregion

		private void Add(Player newPlayer)
		{
			Friends.ToList(friend => friend.NewPlayer(newPlayer));
			UpdateTitle();
		}

		private void Remove(string oldPlayerHash)
		{
			Friends.ToList(friend => friend.RemovedPlayer(oldPlayerHash));
			UpdateTitle();
		}

		private void UpdateTitle()
		{
			var count = Friends.Count(f => f.IsPlaying);
			if(count == 0)
				Title = "friends";
			else
				Title = string.Format("friends({0})", count);
		}

		public void NewFriend()
		{
			IsAdding = true;
		}

		public void CreateFriend()
		{
			if (!string.IsNullOrWhiteSpace(NewFriendName))
			{
				Friends.Add(new Friend(NewFriendName));
				SaveFriends();
				App.Events.Publish(new RepublishFriendsRequest());
			}
			IsAdding = false;
			NewFriendName = "";
		}

		private void SaveFriends()
		{
			UserSettings.Current.Friends.Clear();
			foreach (Friend friend in Friends)
			{
				UserSettings.Current.Friends.Add(friend.Name);
			}
			UserSettings.Current.Save();
		}

		public void CancelNewFriend()
		{
			IsAdding = false;
		}

		public void DeleteFriend(Friend friend)
		{
			foreach(var player in friend.Players)
			{
				App.Events.Publish(new FriendChanged(friend, player, false));
			}
			Friends.Remove(friend);
			App.Events.Publish(new RepublishFriendsRequest());
			SaveFriends();
		}
	}

	public class FriendChanged
	{
		public Friend Friend { get; set; }
		public Player Player { get; set; }
		public bool IsAdded { get; set; }

		public FriendChanged(Friend friend, Player player, bool isAdded)
		{
			Friend = friend;
			Player = player;
			IsAdded = isAdded;
		}
	}

	public class FilterByFriendRequest
	{
		public FilterByFriendRequest(Friend friend, bool isFiltered)
		{
			Friend = friend;
			IsFiltered = isFiltered;
		}

		public Friend Friend { get; set; }
		public bool IsFiltered { get; set; }
	}
}