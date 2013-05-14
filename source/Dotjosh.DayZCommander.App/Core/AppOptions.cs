using System.Runtime.Serialization;

namespace Dotjosh.DayZCommander.App.Core
{
	[DataContract]
	public class AppOptions : BindableBase
	{
		[DataMember] private bool _lowPingRate;

		public bool LowPingRate
		{
			get { return _lowPingRate; }
			set
			{
				_lowPingRate = value;
				PropertyHasChanged("LowPingRate");
				UserSettings.Current.Save();
			}
		}

	}
}