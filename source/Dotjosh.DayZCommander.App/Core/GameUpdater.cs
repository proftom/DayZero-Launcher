using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using SharpCompress.Common;
using SharpCompress.Reader;

// ReSharper disable InconsistentNaming
namespace Dotjosh.DayZCommander.App.Core
{
	public class GameUpdater
	{
		const string _dayZPage = "http://cdn.armafiles.info/latest/";
		public int? LatestArma2OABetaRevision { get; private set; }
		public string LatestArma2OABetaUrl { get; private set; }
		public Version LatestDayZVersion { get; private set; }

		public bool UpdateDayZ()
		{
			if(string.IsNullOrEmpty(LocalMachineInfo.Current.DayZPath))
			{
				return false;
			}
			var dayZFiles = GetDayZFiles();
			foreach(var dayZFile in dayZFiles)
			{
				var dayZAddonPath = Path.Combine(LocalMachineInfo.Current.DayZPath, @"Addons").MakeSurePathExists();
				var dayZFileUrl = Path.Combine(_dayZPage, dayZFile);
				var dayZFilePath = Path.Combine(LocalMachineInfo.Current.DayZPath, dayZFile);
				var webClient = new WebClient();
				webClient.DownloadFile(dayZFileUrl, dayZFilePath);
				if(dayZFile.EndsWithAny("zip", "rar"))
				{
					using(var stream = File.OpenRead(dayZFilePath))
					{
						using(var reader = ReaderFactory.Open(stream))
						{
							while(reader.MoveToNextEntry())
							{
								if(reader.Entry.IsDirectory)
								{
									continue;
								}
								reader.WriteEntryToDirectory(dayZAddonPath, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
							}
						}
					}
					File.Delete(dayZFilePath);
				}
			}
			return true;
		}



		private List<string> GetDayZFiles()
		{
			var files = new List<string>();
			string responseBody;
			if(!HttpGet(_dayZPage, out responseBody))
			{
				return files;
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

			return files;
		}

		public static bool HttpGet(string page, out string responseBody)
		{
			responseBody = null;
			var request = (HttpWebRequest)WebRequest.Create(page);
			request.Method = "GET";
			request.Timeout = 120000; // ms

			try
			{
				using(var response = request.GetResponse())
				{
					using(var responseStream = response.GetResponseStream())
					{
						if(responseStream == null)
						{
							return false;
						}
						var streamReader = new StreamReader(responseStream);
						responseBody = streamReader.ReadToEnd();
						streamReader.Close();
					}
				}
			}
			catch//(Exception e)
			{
				return false;
			}
			return true;
		}
	}
}