using System.Runtime.Serialization;
using System.Windows;
using zombiesnu.DayZeroLauncher.App.Ui;

namespace zombiesnu.DayZeroLauncher.App.Core
{
	[DataContract]
	public class WindowSettings
	{
		[DataMember] public double Top { get; set; } 
		[DataMember] public double Left { get; set; } 
		[DataMember] public double Height { get; set; } 
		[DataMember] public double Width { get; set; }
		[DataMember] public bool IsMaximized { get; set; }

		public static WindowSettings Create(MainWindow mainWindow)
		{
			return new WindowSettings
			{
				Top = mainWindow.Top,
				Left = mainWindow.Left,
				Height = mainWindow.Height,
				Width = mainWindow.Width,
				IsMaximized = mainWindow.WindowState == WindowState.Maximized
			};
		}

		public void Apply(MainWindow mainWindow)
		{
			mainWindow.Top = Top;
			mainWindow.Left = Left;
			mainWindow.Height = Height;
			mainWindow.Width = Width;
			mainWindow.WindowState = IsMaximized ? WindowState.Maximized : WindowState.Normal;
		}
	}
}