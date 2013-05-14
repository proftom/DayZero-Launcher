using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using NLog;
using System.Threading;

namespace Dotjosh.DayZCommander.App.Core
{
	public static class GameLauncher
	{
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		public static void JoinServer(Server server)
		{
			var arguments = new StringBuilder();

			string exePath;
			
			if(UserSettings.Current.GameOptions.LaunchUsingSteam)
			{
				exePath = Path.Combine(LocalMachineInfo.Current.SteamPath, "steam.exe");
				if(!File.Exists(exePath))
				{
					MessageBox.Show("Could not find Steam, please adjust your options or check your Steam installation.");
					return;
				}
				
				 arguments.Append(" -applaunch 33930");

                if(UserSettings.Current.GameOptions.Arma2OASteamUpdate){
                    string mainEXE = Path.Combine(CalculatedGameSettings.Current.Arma2OAPath, @"arma2oa.exe");
                    string betaEXE = Path.Combine(CalculatedGameSettings.Current.Arma2OAPath, @"Expansion\beta\arma2oa.exe");
					if(File.Exists(mainEXE) && File.Exists(betaEXE))
					{
						var mainExeVersion = FileVersionInfo.GetVersionInfo(mainEXE).ProductVersion;
						var betaExeVersion = FileVersionInfo.GetVersionInfo(betaEXE).ProductVersion;

						if (mainExeVersion != betaExeVersion)
						{
							File.Copy(mainEXE, mainEXE + "_" + mainExeVersion, true);
							File.Copy(betaEXE, mainEXE, true);
						}
					}
                }

			}
			else
			{
				exePath = CalculatedGameSettings.Current.Arma2OAExePath;
			}

			if(UserSettings.Current.GameOptions.MultiGpu)
			{
				arguments.Append(" -winxp");
			}

			if(UserSettings.Current.GameOptions.WindowedMode)
			{
				arguments.Append(" -window");
			}

			if(!string.IsNullOrWhiteSpace(UserSettings.Current.GameOptions.AdditionalStartupParameters))
			{
				arguments.Append(" " + UserSettings.Current.GameOptions.AdditionalStartupParameters);
			}

			arguments.Append(" -noSplash -noFilePatching");
			arguments.Append(" -connect=" + server.IpAddress);
			arguments.Append(" -port=" + server.Port);
			arguments.AppendFormat(" \"-mod={0};expansion;expansion\\beta;expansion\\beta\\expansion;{1}\"", CalculatedGameSettings.Current.Arma2Path, CalculatedGameSettings.Current.DayZPath);

			try
			{
				var p = new Process
			        		{
			        			StartInfo =
			        				{
			        					FileName = exePath,
			        					Arguments = arguments.ToString(),
			        					WorkingDirectory = CalculatedGameSettings.Current.Arma2OAPath,
			        					UseShellExecute = true,
			        				}
			        		};
				p.Start();

				UserSettings.Current.AddRecent(server);

                if(UserSettings.Current.GameOptions.CloseDayZCommander){
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
			}
			catch(Exception ex)
			{
				//var joinServerException = new JoinServerException(exePath, arguments.ToString(), CalculatedGameSettings.Current.Arma2OAPath, ex);
				//_logger.Error(joinServerException);
			}
			finally
			{
				arguments.Clear();
			}
		}
	}

	public class JoinServerException : Exception
	{
		public JoinServerException(string fileName, string arguments, string workingDirectory, Exception exception) : base(
				"There was an error launching the game.\r\n"
				+ "File Name:" + fileName + "\r\n"
				+ "Arguments:" + arguments + "\r\n"
				+ "Working Directory:" + workingDirectory,
			exception)
		{

		}
	}
}