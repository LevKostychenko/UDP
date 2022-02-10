using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace UDP.Client.Services
{
    public class NetworkService
    {
        private const string MULTICAST_IP = "224.5.6.7";
        private const int REMOTE_PORT = 8001;
        private const string DISCOVERY_REQUEST = "$DISC~";
        private const string DISCOVERY_RESPONSE = "#DISC~";

        public int GetFreeLocalPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return port;
        }

        public IPAddress GetMulticastAddress()
            => IPAddress.Parse(MULTICAST_IP);

        public IPAddress GetBroadcastAddress()
            => IPAddress.Broadcast;       

        public int GetRemotePort()
            => REMOTE_PORT;

        public string GetDiscoveryRequestMessage()
            => DISCOVERY_REQUEST;

        public string GetDiscoveryResponseMessage()
            => DISCOVERY_RESPONSE;

        public bool IsDiscoveryRequestMessage(string message)
            => message.Split(" ")
                .Last()
                .Equals(
                    DISCOVERY_REQUEST, 
                    System.StringComparison.OrdinalIgnoreCase);

        public bool IsDiscoveryResponseMessage(string message)
            => message.Split(" ")
                .Last()
                .Equals(
                    DISCOVERY_RESPONSE,
                    System.StringComparison.OrdinalIgnoreCase);        
    }
}
