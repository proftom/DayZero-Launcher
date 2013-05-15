using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using zombiesnu.DayZeroLauncher.App.Core;

namespace zombiesnu.DayZeroLauncher.App.Ui.Converters
{
	public class UpdateStatusToForegroundConverter : IValueConverter
	{
		private static SolidColorBrush InProgress = new SolidColorBrush(Colors.LightGreen);
		private static SolidColorBrush ActionRequired = new SolidColorBrush(Colors.Yellow);
		private static SolidColorBrush OK = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
		private static SolidColorBrush Default = new SolidColorBrush(Colors.Red);

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if((string) value == DayZeroLauncherUpdater.STATUS_CHECKINGFORUPDATES)
			{
				return InProgress;
			}
			if((string) value == DayZeroLauncherUpdater.STATUS_DOWNLOADING)
			{
				return InProgress;
			}
			if((string) value == DayZeroLauncherUpdater.STATUS_RESTARTTOAPPLY
				|| (string)value == DayZeroLauncherUpdater.STATUS_OUTOFDATE)
			{
				return ActionRequired;
			}
			if((string) value == DayZeroLauncherUpdater.STATUS_UPTODATE)
			{
				return OK;
			}

			return Default;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}