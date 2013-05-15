using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace zombiesnu.DayZeroLauncher.App.Ui.Converters
{
	public class IsPlayingToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if((bool)value)
				return new SolidColorBrush(Colors.White);
			return new SolidColorBrush(Color.FromArgb(255, 130, 130, 130));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}