using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using SharpCompress.Common;
using SharpCompress.Reader;

namespace Dotjosh.DayZCommander.App.Core
{
	public class Arma2Installer : BindableBase
	{
		private string _latestDownloadUrl;
		private string _downloadedFileLocation;
		private string _extractedLocation;
		private string _status;
		private bool _isRunning;

		public void DownloadAndInstall(string latestDownloadUrl)
		{
			_latestDownloadUrl = latestDownloadUrl;

			if(string.IsNullOrEmpty(CalculatedGameSettings.Current.Arma2OAPath)
			   || string.IsNullOrEmpty(_latestDownloadUrl))
			{
				return;
			}
			var latestArma2OABetaFile = Path.GetFileName(_latestDownloadUrl);
			if(string.IsNullOrEmpty(latestArma2OABetaFile))
			{
				return;
			}

			IsRunning = true;
			Status = "Downloading... 0%";

			_downloadedFileLocation = Path.Combine(CalculatedGameSettings.Current.Arma2OAPath, latestArma2OABetaFile);;
			var extension = Path.GetExtension(latestArma2OABetaFile);
			_extractedLocation = Path.Combine(CalculatedGameSettings.Current.Arma2OAPath, latestArma2OABetaFile.Replace(extension, "") + ".unpacked");

//			if(File.Exists(_downloadedFileLocation))
//			{
//				Status = "Downloading... 100%";
//				ExtractFile();
//				IsRunning = false;
//				return;
//			}

			using(var webClient = new WebClient())
			{
				webClient.DownloadProgressChanged += (sender, args) =>
				                                     	{
				                                     		Status = string.Format("Downloading... {0}%", args.ProgressPercentage);
				                                     	};
				webClient.DownloadFileCompleted += (sender, args) =>
				                                   	{
				                                   		if(args.Error != null)
				                                   		{
				                                   			Status = "Error downloading";
				                                   			IsRunning = false;
				                                   			return;
				                                   		}
				                                   		ExtractFile();
				                                   	};
				webClient.DownloadFileAsync(new Uri(_latestDownloadUrl), _downloadedFileLocation);
			}
		}

		public bool IsRunning
		{
			get { return _isRunning; }
			set
			{
				_isRunning = value;
				PropertyHasChanged("IsRunning");
			}
		}

		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				Execute.OnUiThread(() => PropertyHasChanged("Status"));
			}
		}

		private void ExtractFile()
		{
			new Thread(() =>
			           	{
			           		try
			           		{
			           			Status = "Extracting";
			           			Directory.CreateDirectory(_extractedLocation);
			           			using (var stream = File.OpenRead(_downloadedFileLocation))
			           			{
			           				using (var reader = ReaderFactory.Open(stream))
			           				{
			           					while (reader.MoveToNextEntry())
			           					{
			           						if (reader.Entry.IsDirectory)
			           						{
			           							continue;
			           						}
			           						var fileName = Path.GetFileName(reader.Entry.FilePath);
			           						if (string.IsNullOrEmpty(fileName))
			           						{
			           							continue;
			           						}
			           						reader.WriteEntryToDirectory(_extractedLocation,
			           						                             ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
			           						if (fileName.EndsWith(".exe"))
			           						{
			           							var p = new Process
			           							        	{
			           							        		StartInfo =
			           							        			{
			           							        				CreateNoWindow = false,
			           							        				UseShellExecute = true,
			           							        				WorkingDirectory = _extractedLocation,
			           							        				FileName = Path.Combine(_extractedLocation, fileName)
			           							        			}
			           							        	};
			           							p.Start();
			           							Status = "Installing";
			           							p.WaitForExit();
			           						}
			           					}
			           				}
			           			}

			           			Status = "Install complete";
			           		}
			           		catch(Exception ex)
			           		{
			           			Status = "Coult not complete";
			           			IsRunning = false;
			           		}

			           		try
			           		{
			           			File.Delete(_downloadedFileLocation);
                                Directory.Delete(_extractedLocation, true);
			           		}
			           		catch(Exception ex)
			           		{
								
			           		}
			           		IsRunning = false;
			           	}).Start();

		}
	}
}