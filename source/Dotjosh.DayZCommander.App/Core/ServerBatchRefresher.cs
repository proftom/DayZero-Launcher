using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Dotjosh.DayZCommander.App.Core
{
	public class ServerBatchRefresher : BindableBase
	{
		public string InProgressText { get; set; }
		private bool _isUpdating;
		private readonly ICollection<Server> _items;
		private int _processed;
		private int _processedServersCount;
		public event Action RefreshAllComplete;

		public ServerBatchRefresher(string inProgressText, ICollection<Server> items)
		{
			_items = items;
			InProgressText = inProgressText;
		}

		public int UnprocessedServersCount
		{
			get { return _items.Count - ProcessedServersCount; }
		}

		public int TotalCount
		{
			get { return _items.Count; }
		}

		public int ProcessedServersCount
		{
			get { return _processedServersCount; }
			set
			{
				_processedServersCount = value;
				PropertyHasChanged("ProcessedServersCount", "UnprocessedServersCount");
			}
		}

		public void RefreshAll()
		{
			if(_isUpdating)
				return;

			object incrementLock = new object();

			_isUpdating = true;
			ProcessedServersCount = 0;

			_processed = 0;
			var totalCount = _items.Count;

			var t = new Thread(() =>
			                   	{
			                   		try
			                   		{
			                   			while(_processed <= totalCount)
			                   			{
			                   				Execute.OnUiThread(() =>
			                   				                   	{
			                   				                   		ProcessedServersCount = _processed;
			                   				                   	});
			                   				Thread.Sleep(150);
			                   				if(_processed == totalCount)
			                   				{
			                   					_isUpdating = false;
			                   					break;
			                   				}
			                   			}
			                   		}
			                   		finally
			                   		{
			                   			Execute.OnUiThread(() =>
			                   			                   	{
			                   			                   		ProcessedServersCount = totalCount;
			                   			                   	});
										if(RefreshAllComplete != null)
											RefreshAllComplete();
			                   			_isUpdating = false;
			                   		}
			                   	});
			t.IsBackground = true;
			t.Start();

			var serverUpdates = _items
				.Select<Server, Action<Action>>(server => 
				                                onComplete => 
				                                server.BeginUpdate(doubleDispatchServer =>
				                                                   	{
				                                                   		try
				                                                   		{
				                                                   			onComplete();
				                                                   		}
				                                                   		finally
				                                                   		{
				                                                   			lock (incrementLock)
				                                                   			{
				                                                   				_processed++;
				                                                   			}
				                                                   		}
				                                                   	}))
				.ToArray();
			ServerRefreshQueue.Instance.Enqueue(serverUpdates);			
		}
	}
}