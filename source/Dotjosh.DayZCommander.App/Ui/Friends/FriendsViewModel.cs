using System.ComponentModel;

namespace Dotjosh.DayZCommander.App.Ui.Friends
{
	public class FriendsViewModel : ViewModelBase
	{
		public FriendsViewModel()
		{
			Title = "friends";

			ManageViewModel = new ManageViewModel();
			ManageViewModel.PropertyChanged += (sender, args) =>
			{
				SyncTitle(args);
			};
			ListViewModel = new ListViewModel();
		}

		private void SyncTitle(PropertyChangedEventArgs args)
		{
			if(args.PropertyName == "Title")
			{
				Title = ManageViewModel.Title;
			}
		}

		public ManageViewModel ManageViewModel { get; set; }
		public ListViewModel ListViewModel { get; set; }

//		private bool Filter(object o)
//		{
//		
//			var server = (Server)o;
//
////			if(_friendFilter.Count > 0)
////				return _friendFilter
////							.SelectMany(f => f.Players)
////							.Any(x => x.Server.IpAddress == server.IpAddress && x.Server.Port == server.Port);
//
//			if(_filter != null)
//				return _filter(server);
//
//			return true;
//		}
	}
}