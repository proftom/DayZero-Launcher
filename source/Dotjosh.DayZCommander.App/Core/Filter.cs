using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Timers;
using Dotjosh.DayZCommander.App.Ui.ServerList;

namespace Dotjosh.DayZCommander.App.Core
{
	[DataContract]
	public class Filter : BindableBase
	{
		[DataMember] private long ?_maxPing;
		[DataMember] private string _name;
		[DataMember] private bool _hideEmpty;
		[DataMember] private bool _hideFull;
		[DataMember] private bool _supressPublish;
		[DataMember] private string _timeOfDay;
		[DataMember] private bool _hideUnresponsive;
		[DataMember] private bool _hidePasswordProtected;
		[DataMember] private bool _hideWrongArma2Version;
		[DataMember] private bool _hideWrongDayZVersion;
		[DataMember] private bool? _hasScores;
		[DataMember] private bool? _hasDeathMessages;
		[DataMember] private bool? _hasArmor;
		[DataMember] private bool? _hasThirdPerson;
		[DataMember] private bool? _hasTracers;
		[DataMember] private bool? _hasNameplates;
		[DataMember] private bool? _hasCrosshairs;
		private Timer _publishTimer;

		public Filter()
		{
			LoadDefaults();
		}

		public void LoadDefaults()
		{
			_supressPublish = true;
			MaxPing = null;
			Name = null;
			HideEmpty = false;
			HideUnresponsive = true;
			HideFull = false;
			TimeOfDay = "Any time of day";
			HidePasswordProtected = true;
			_supressPublish = false;
		}

		public long? MaxPing
		{
			get { return _maxPing; }
			set
			{
				if(value == 0)
					_maxPing = null;
				else
					_maxPing = value;
				PropertyHasChanged("MaxPing");
				PublishFilter();
			}
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				PropertyHasChanged("Name");
				PublishFilter();
			}
		}

		public string TimeOfDay
		{
			get { return _timeOfDay; }
			set
			{
				_timeOfDay = value;
				PropertyHasChanged("TimeOfDay");
				PublishFilter();
			}
		}

		public IEnumerable<string> TimeOfDayOptions
		{
			get
			{
				yield return "Any time of day";
				yield return "Night only";
				yield return "Day only";
			}
		}

		public bool HideEmpty
		{
			get { return _hideEmpty; }
			set
			{
				_hideEmpty = value;
				PropertyHasChanged("HideEmpty");
				PublishFilter();
			}
		}

		public bool HideUnresponsive
		{
			get { return _hideUnresponsive; }
			set
			{
				_hideUnresponsive = value;
				PropertyHasChanged("HideUnresponsive");
				PublishFilter();
			}
		}

		public bool HideFull
		{
			get { return _hideFull; }
			set
			{
				_hideFull = value;
				PropertyHasChanged("HideFull");
				PublishFilter();
			}
		}

		public bool? HasArmor
		{
			get { return _hasArmor; }
			set
			{
				_hasArmor = value;
				PropertyHasChanged("HasArmor");
				PublishFilter();
			}
		}

		public bool? HasThirdPerson
		{
			get { return _hasThirdPerson; }
			set
			{
				_hasThirdPerson = value;
				PropertyHasChanged("HasThirdPerson");
				PublishFilter();
			}
		}

		public bool? HasTracers
		{
			get { return _hasTracers; }
			set
			{
				_hasTracers = value;
				PropertyHasChanged("HasTracers");
				PublishFilter();
			}
		}

		public bool? HasNameplates
		{
			get { return _hasNameplates; }
			set
			{
				_hasNameplates = value;
				PropertyHasChanged("HasNameplates");
				PublishFilter();
			}
		}

		public bool? HasCrosshairs
		{
			get { return _hasCrosshairs; }
			set
			{
				_hasCrosshairs = value;
				PropertyHasChanged("HasCrosshairs");
				PublishFilter();
			}
		}

		public bool? HasDeathMessages
		{
			get { return _hasDeathMessages; }
			set
			{
				_hasDeathMessages = value;
				PropertyHasChanged("HasDeathMessages");
				PublishFilter();
			}
		}

		public bool? HasScores
		{
			get { return _hasScores; }
			set
			{
				_hasScores = value;
				PropertyHasChanged("HasScores");
				PublishFilter();
			}
		}

		public bool HidePasswordProtected
		{
			get { return _hidePasswordProtected; }
			set
			{
				_hidePasswordProtected = value;
				PropertyHasChanged("HidePasswordProtected");
				PublishFilter();
			}
		}

		public bool HideWrongArma2Version
		{
			get { return _hideWrongArma2Version; }
			set
			{
				_hideWrongArma2Version = value;
				PropertyHasChanged("HideWrongArma2Version");
				PublishFilter();
			}
		}

		public bool HideWrongDayZVersion
		{
			get { return _hideWrongDayZVersion; }
			set
			{
				_hideWrongDayZVersion = value;
				PropertyHasChanged("HideWrongDayZVersion");
				PublishFilter();
			}
		}

		public void PublishFilter()
		{
			if(_supressPublish)
				return;

			UserSettings.Current.Filter = this;
			UserSettings.Current.Save();

			if(_publishTimer == null)
			{
				_publishTimer = new Timer(300);
				_publishTimer.Elapsed += PublishTimerOnElapsed;
			}

			_publishTimer.Stop();
			_publishTimer.Start();

		}

		private void PublishTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			_publishTimer.Stop();
			Execute.OnUiThread(() =>
			{
				App.Events.Publish(new FilterUpdated(s => FilterHandler(s)));
			});
		}

		private bool FilterHandler(Server s)
		{
			if(MaxPing != null && s.Ping > MaxPing)
				return false;

			if(!string.IsNullOrWhiteSpace(Name))
			{
				if(s.Name == null || s.Name.IndexOf(Name, StringComparison.CurrentCultureIgnoreCase) == -1)
					return false;
			}

			if(HideEmpty && s.CurrentPlayers == 0)
				return false;

			if(HideFull && s.FreeSlots == 0)
				return false;

			if(TimeOfDay == "Night only")
			{
				if(s.IsNight == null || s.IsNight == false)
					return false;
			}

			if(TimeOfDay == "Day only")
			{
				if(s.IsNight == null || s.IsNight == true)
					return false;							
			}

			if(HasArmor != null)
			{
				if(HasArmor == true && !s.Info.Armor.Enabled)
					return false;				
				if(HasArmor == false && s.Info.Armor.Enabled)
					return false;
			}

			if(HasThirdPerson != null)
			{
				if(HasThirdPerson == true && !s.Info.ThirdPerson.Enabled)
					return false;				
				if(HasThirdPerson == false && s.Info.ThirdPerson.Enabled)
					return false;
			}

			if(HasTracers != null)
			{
				if(HasTracers == true && !s.Info.Tracers.Enabled)
					return false;				
				if(HasTracers == false && s.Info.Tracers.Enabled)
					return false;
			}

			if(HasNameplates != null)
			{
				if(HasNameplates == true && !s.Info.Nameplates.Enabled)
					return false;				
				if(HasNameplates == false && s.Info.Nameplates.Enabled)
					return false;
			}

			if(HasCrosshairs != null)
			{
				if(HasCrosshairs == true && !s.Info.Crosshairs.Enabled)
					return false;				
				if(HasCrosshairs == false && s.Info.Crosshairs.Enabled)
					return false;
			}

			if(HasDeathMessages != null)
			{
				if(HasDeathMessages == true && !s.Info.DeathMessages.Enabled)
					return false;				
				if(HasDeathMessages == false && s.Info.DeathMessages.Enabled)
					return false;
			}

			if(HasScores != null)
			{
				if(HasScores == true && !s.Info.Scores.Enabled)
					return false;				
				if(HasScores == false && s.Info.Scores.Enabled)
					return false;
			}

			if(HideUnresponsive && s.LastException != null)
			{
				return false;
			}

			if(HidePasswordProtected && s.HasPassword)
			{
				return false;
			}

			if(HideWrongArma2Version)
			{
				if(!s.IsSameArma2OAVersion)
				{
					return false;
				}
			}

			if(HideWrongDayZVersion)
			{
				if(!s.IsSameDayZVersion)
				{
					return false;
				}
			}

			return true;			
		}
	}
}