using System.Collections.Generic;
using System.Runtime.Serialization;
namespace zombiesnu.DayZeroLauncher.App.Core
{
	[DataContract]
	public class FavoriteServer
	{
		[DataMember] private readonly string _ipAddress;
		[DataMember] private readonly int _port;
		[DataMember] private readonly string _name;

		public FavoriteServer(Server server)
		{
			_ipAddress = server.IpAddress;
			_port = server.Port;
			_name = server.Name;
		}

		public bool Matches(Server server)
		{
			return server.IpAddress == _ipAddress && server.Port == _port;
		}
        /*
		public Server CreateServer()
		{
			
            var server = new Server(_ipAddress, _port);
			server.Settings = new SortedDictionary<string, string>()
			                  	{
			                  		{"hostname",_name}
			                  	};
			return server;
		}
         */
	}
}