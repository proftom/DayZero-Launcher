using System;
using System.Globalization;
using System.Windows.Data;

namespace Dotjosh.DayZCommander.App.Ui.Converters
{
	public class CountsToPercentageConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			int count = (int)values[0];
			int totalCount = (int)values[1];
			decimal percentage = ((decimal)count/(decimal)totalCount);
			var roundedPercentage = Math.Round(percentage*100);
			if(roundedPercentage == 0)
				return "< 1%";
			return roundedPercentage + "%";
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}