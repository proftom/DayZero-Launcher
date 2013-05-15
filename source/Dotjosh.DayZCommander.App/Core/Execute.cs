using System;
using System.Windows;
using System.Windows.Threading;

namespace zombiesnu.DayZeroLauncher.App.Core
{
	public class Execute
	{
		public static void OnUiThread(Action action)
		{
			if(Application.Current !=  null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
		} 
	}
}