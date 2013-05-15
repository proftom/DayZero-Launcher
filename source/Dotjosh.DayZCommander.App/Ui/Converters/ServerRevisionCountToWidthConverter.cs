using System;
using System.Globalization;
using System.Windows.Data;

namespace zombiesnu.DayZeroLauncher.App.Ui.Converters
{
	public class ServerRevisionCountToWidthConverter : IMultiValueConverter
	{
		public double MaxWidth = 140;

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			int count = (int)values[0];
			int totalCount = (int)values[1];
			decimal percentage = ((decimal)count/(decimal)totalCount);
			return (double)Math.Floor((decimal)MaxWidth*percentage);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}