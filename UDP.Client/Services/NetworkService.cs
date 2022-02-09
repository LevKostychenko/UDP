using System.Net;
using System.Net.Sockets;

namespace UDP.Client.Services
{
    public class NetworkService
    {
        private const string MULTICAST_IP = "224.5.6.7";
        private const int REMOTE_PORT = 8001;

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

        public string GetLocalIp()
        {
            var localIp = string.Empty;
            var host = Dns.GetHostEntry(
                Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = ip.ToString();
                    break;
                }
            }

            return localIp;
        }

        public int GetRemotePort()
            => REMOTE_PORT;
    }
}
