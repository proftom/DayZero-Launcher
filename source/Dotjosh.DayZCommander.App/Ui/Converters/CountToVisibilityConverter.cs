using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dotjosh.DayZCommander.App.Ui.Converters
{
	public class CountToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value != null && int.Parse(System.Convert.ToString(value)) > 0)
				return Visibility.Visible;

			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}