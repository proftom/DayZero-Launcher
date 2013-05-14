namespace Dotjosh.DayZCommander.App.Ui.ServerList
{
	public class ServerListViewModel : ViewModelBase
	{
		public ServerListViewModel()
		{
			Title = "servers";

			FiltersViewModel = new FiltersViewModel();
			ListViewModel = new ListViewModel();

			FiltersViewModel.Filter.PublishFilter();
		}

		public FiltersViewModel FiltersViewModel { get; set; }
		public ListViewModel ListViewModel { get; set; }
	}
}