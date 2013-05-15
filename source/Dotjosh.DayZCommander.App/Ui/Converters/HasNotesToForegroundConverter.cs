using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace zombiesnu.DayZeroLauncher.App.Ui.Converters
{
	public class HasNotesToForegroundConverter : IValueConverter
	{
		private static readonly SolidColorBrush _true = new SolidColorBrush(Colors.White);
		private static readonly SolidColorBrush _false = new SolidColorBrush(Color.FromArgb(255, 130, 130, 130));

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if((bool)value)
				return _true;
			return _false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}