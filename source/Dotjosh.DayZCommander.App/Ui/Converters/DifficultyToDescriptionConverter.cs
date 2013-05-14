using System;
using System.Globalization;
using System.Windows.Data;

namespace Dotjosh.DayZCommander.App.Ui.Converters
{
	public class DifficultyToDescriptionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string nullText = System.Convert.ToString(parameter);

			var val = (int?) value;
			if(val == null)
				return nullText;
			if(val == 0)
				return "Recruit";
			if(val == 1)
				return "Regular";
			if(val == 2)
				return "Veteran";
			if(val == 3)
				return "Expert";

			return nullText;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}