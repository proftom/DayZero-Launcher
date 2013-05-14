using System;
using System.Globalization;
using System.Windows.Data;
using Dotjosh.DayZCommander.App.Core;

namespace Dotjosh.DayZCommander.App.Ui.Converters
{
	public class TimeToTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var dateTime = value as DateTime?;
			if (value == null)
				return "24/7 Day";

			var format = UserSettings.Current.GameOptions.TwentyFourHourTimeFormat
				? "HH:mm"
				: "h:mm tt";
			return dateTime.Value.ToString(format);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}