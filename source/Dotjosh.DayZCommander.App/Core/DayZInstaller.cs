using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Dotjosh.DayZCommander.App.Ui;
using SharpCompress.Common;
using SharpCompress.Reader;

namespace Dotjosh.DayZCommander.App.Core
{
	public class DayZInstaller : ViewModelBase
	{
		private string _latestDownloadUrl;
		private string _status;
		private bool _isRunning;
		private string _currentDayZFile;
		private string _currentDayZFilePath;
		private int? _currentIndex;
		private List<string> _dayZFiles;

		public void DownloadAndInstall(string latestDownloadUrl)
		{
			_latestDownloadUrl = latestDownloadUrl;

			if(string.IsNullOrEmpty(CalculatedGameSettings.Current.DayZPath)
			   || string.IsNullOrEmpty(_latestDownloadUrl))
			{
				return;
			}

			CalculatedGameSettings.Current.DayZPath.MakeSurePathExists();

			IsRunning = true;
			Status = "Getting file list...";

			new Thread(GetDayZFiles).Start();
		}

		private void ProcessNext()
		{
			if(_currentIndex == null)
				_currentIndex = 0;
			else
				_currentIndex++;

			if(_dayZFiles.Count <= _currentIndex+1)
			{
				Status = "Update complete";
				return;
			}

			_currentDayZFile = _dayZFiles[(int) _currentIndex];

			Status = string.Format("Downloading file {0} of {1}... 0%", _currentIndex+1, _dayZFiles.Count);


			var dayZFileUrl = Path.Combine(_latestDownloadUrl, _currentDayZFile);
			_currentDayZFilePath = Path.Combine(CalculatedGameSettings.Current.DayZPath, _currentDayZFile);
			var webClient = new WebClient();
			webClient.DownloadProgressChanged += (sender, args) =>
			                                     	{
														Status = string.Format("Downloading file {0} of {1}... {0}%", _currentIndex+1, _dayZFiles.Count, args.ProgressPercentage);
			                                     	};
			webClient.DownloadFileCompleted += FileDownloaded;
			webClient.DownloadFileAsync(new Uri(dayZFileUrl), _currentDayZFilePath);
		}

		private void FileDownloaded(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
		{
			if(asyncCompletedEventArgs.Error != null)
			{
				Status = "Error downloading file " + asyncCompletedEventArgs.Error;
				return;
			}

			new Thread(() =>
			           	{
			           		var dayZAddonPath =
			           			Path.Combine(CalculatedGameSettings.Current.DayZPath, @"Addons").MakeSurePathExists();

							Status = string.Format("Extracting file {0} of {1}...", _currentIndex+1, _dayZFiles.Count);

							try
							{
								if (_currentDayZFile.EndsWithAny("zip", "rar"))
								{
									using (var stream = File.OpenRead(_currentDayZFilePath))
									{
										using (var reader = ReaderFactory.Open(stream))
										{
											while (reader.MoveToNextEntry())
											{
												if (reader.Entry.IsDirectory)
												{
													continue;
												}
												reader.WriteEntryToDirectory(dayZAddonPath,
												                             ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
											}
										}
									}
									File.Delete(_currentDayZFilePath);
								}
								ProcessNext();
							}
							catch(Exception ex)
							{
								Status = "Error extracting " + ex;
							}
			           	})
					.Start();
		}

		private void GetDayZFiles()
		{
			var files = new List<string>();
			string responseBody;
			if(!GameUpdater.HttpGet(_latestDownloadUrl, out responseBody))
			{
				Status = "Error getting files";
				return;
			}
			var fileMatches = Regex.Matches(responseBody, @"<a\s+href\s*=\s*(?:'|"")([^'""]+\.[^'""]{3})(?:'|"")", RegexOptions.IgnoreCase);
			foreach(Match match in fileMatches)
			{
				if(!match.Success)
				{
					continue;
				}
				var file = match.Groups[1].Value;
				if(string.IsNullOrEmpty(file))
				{
					continue;
				}

				files.Add(file);
			}
			_dayZFiles = files;

			ProcessNext();
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
	}
}