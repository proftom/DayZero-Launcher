using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace zombiesnu.DayZeroLauncher.App.Ui.Converters
{
	public class BooleanToVisibilityCollapsedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool inverse = parameter != null && bool.Parse((string) parameter);
			bool val = inverse
			           	? !(bool)value
			           	: (bool)value;

			if(val)
				return Visibility.Visible;
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}