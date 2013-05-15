using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace zombiesnu.DayZeroLauncher.App.Ui.Converters
{
	public class EnabledSettingToForegroundConverter : IValueConverter
	{
		public static SolidColorBrush Empty = new SolidColorBrush(Colors.Transparent);
		public static SolidColorBrush Enabled = new SolidColorBrush(Color.FromArgb(255, 238, 238, 238));
		public static SolidColorBrush Disabled = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));

		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value == null)
				return Empty;

			if((bool)value)
			{
				return Enabled;
			}
			return Disabled;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}