using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dotjosh.DayZCommander.App.Ui.Converters
{
	public class NotNullToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null
			       	? Visibility.Visible
			       	: Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}