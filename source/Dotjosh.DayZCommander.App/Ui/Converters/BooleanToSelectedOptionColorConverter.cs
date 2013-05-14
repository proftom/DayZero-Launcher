using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Dotjosh.DayZCommander.App.Ui.Converters
{
	public class BooleanToSelectedOptionColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if((bool)value)
				return new SolidColorBrush(Colors.White);
			return new SolidColorBrush(Color.FromArgb(255, 170, 170, 170));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}