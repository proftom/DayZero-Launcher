using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Caliburn.Micro;
using zombiesnu.DayZeroLauncher.App.Ui;
using zombiesnu.DayZeroLauncher.App.Ui.Controls;

namespace zombiesnu.DayZeroLauncher.App.Core
{
	public class ServerList : ViewModelBase,
		IHandle<RefreshServerRequest>
	{
		private bool _downloadingServerList;
		private ObservableCollection<Server> _items;

		public ServerList()
		{
			Items = new ObservableCollection<Server>();
		}

		private ServerBatchRefresher _refreshAllBatch;
		public ServerBatchRefresher RefreshAllBatch
		{
			get { return _refreshAllBatch; }
			private set
			{
				_refreshAllBatch = value;
				PropertyHasChanged("RefreshAllBatch");
			}
		}

		public ObservableCollection<Server> Items
		{
			get { return _items; }
			private set
			{
				_items = value;
				PropertyHasChanged("Items");
			}
		}

		public bool DownloadingServerList
		{
			get { return _downloadingServerList; }
			set
			{
				_downloadingServerList = value;
				PropertyHasChanged("DownloadingServerList");
			}
		}

		public void GetAndUpdateAll()
		{
			GetAll(
				() => UpdateAll() 
				);
		}

		public void GetAll(Action uiThreadOnComplete)
		{
			DownloadingServerList = true;
			new Thread(() =>
			                    {
			                        var servers = GetAllSync();
									Execute.OnUiThread(() =>
														{
															Items = new ObservableCollection<Server>(servers);
															DownloadingServerList = false;
															uiThreadOnComplete();
														});

			                    }).Start();
		}

		private List<Server> GetAllSync()
		{
			ExecuteGSList("-u");
			return ExecuteGSList("-n arma2oapc -f \"mod LIKE '%@dayzero%'\" -X \\hostname")
				.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
				.Select(line =>
				{
					var indexOfFirstSpace = line.IndexOf(" ");
					var fullIpAddressWithPort = line.Substring(0, indexOfFirstSpace).Split(':');
					var server = new Server(fullIpAddressWithPort[0], fullIpAddressWithPort[1].TryInt());

					server.Settings = new SortedDictionary<string, string>
					{
						{ "hostname", line.Substring(indexOfFirstSpace + 11) }
					};

					return server;
				}
				)
				.ToList();
		}

		public void UpdateAll()
		{
			var batch = new ServerBatchRefresher("Refreshing all servers...", Items);
			App.Events.Publish(new RefreshServerRequest(batch));
		}

		private static string ExecuteGSList(string arguments)
		{
			var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			currentDirectory = new DirectoryInfo(currentDirectory).FullName;

			var p = new Process
			{
				StartInfo =
					{
						UseShellExecute = false,
						CreateNoWindow = true,
						WindowStyle = ProcessWindowStyle.Hidden,
						RedirectStandardOutput = true,
						FileName = Path.Combine(currentDirectory, @"GSList\gslist.exe"),
						Arguments = arguments
					}
			};
			p.Start();
			string output = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			return output;
		}

		private bool _isRunningRefreshBatch;
		public void Handle(RefreshServerRequest message)
		{
			if(_isRunningRefreshBatch)
				return;

			_isRunningRefreshBatch = true;
			App.Events.Publish(new RefreshingServersChange(true));
			RefreshAllBatch = message.Batch;
			RefreshAllBatch.RefreshAllComplete += RefreshAllBatchOnRefreshAllComplete;
			RefreshAllBatch.RefreshAll();
		}

		private void RefreshAllBatchOnRefreshAllComplete()
		{
			RefreshAllBatch.RefreshAllComplete -= RefreshAllBatchOnRefreshAllComplete;
			_isRunningRefreshBatch = false;
			Execute.OnUiThread(() =>
			                   	{
			                   		App.Events.Publish(new RefreshingServersChange(false));
			                   	});
		}
	}

	public class RefreshingServersChange
	{
		public bool IsRunning { get; set; }

		public RefreshingServersChange(bool isRunning)
		{
			IsRunning = isRunning;
		}
	}
}