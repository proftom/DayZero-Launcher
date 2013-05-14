using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Dotjosh.DayZCommander.App.Core
{
	public static class GameVersions
	{
		public static string BuildArma2OAExePath(string arma2OAPath)
		{
			return Path.Combine(arma2OAPath, @"arma2oa.exe");
		}

		public static Version ExtractArma2OABetaVersion(string arma2OAExePath)
		{
			if(!File.Exists(arma2OAExePath))
				return null;

			var versionInfo = FileVersionInfo.GetVersionInfo(arma2OAExePath);
			Version version;
			if(Version.TryParse(versionInfo.ProductVersion, out version))
			{
				return version;
			}
			return null;
		}

		public static Version ExtractDayZVersion(string dayZPath)
		{
			var dayz_code_file = Path.Combine(dayZPath, @"addons\dayz_code.pbo");
			if(!File.Exists(dayz_code_file))
			{
				return null;
			}
			var dayz_code_file_lines = File.ReadAllLines(dayz_code_file);
			foreach(var changeLogLine in dayz_code_file_lines)
			{
				var match = Regex.Match(changeLogLine, @"\x01\x00version\x00(?<Version>\d(?:\.\d){1,3})", RegexOptions.IgnoreCase);
				if(!match.Success)
				{
					continue;
				}
				Version version;
				var versionMatch = match.Groups["Version"];
				if(!versionMatch.Success)
					continue;

				if(Version.TryParse(versionMatch.Value, out version))
				{
					return version;
				}
			}
			return null;
		}
	}
}