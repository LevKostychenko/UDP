using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UDP.Client.Services;

namespace UDP.Client.Extensions
{
    internal static class UdpClientExtensions
    {
        public static async Task SendBroadcastMessageAsync(
            this UdpClient client,
            string message,
            string userName)
        {
            var service = new NetworkService();

            var endPoint = new IPEndPoint(
                service.GetBroadcastAddress(),
                service.GetRemotePort());

            await SendMessageAsync(
                    message,
                    userName,
                    client,
                    endPoint);
        }

        public static async Task SendMulticastMessageAsync(
            this UdpClient client,
            string message,
            string userName)
        {
            var service = new NetworkService();

            var endPoint = new IPEndPoint(
                service.GetMulticastAddress(),
                service.GetRemotePort());

            await SendMessageAsync(
                    message,
                    userName,
                    client,
                    endPoint);
        }

        private static async Task SendMessageAsync(
            string message,
            string userName,
            UdpClient client,
            IPEndPoint endPoint)
        {
            message = string.Format("{0}: {1}", userName, message);
            byte[] data = Encoding.Unicode.GetBytes(message);
            await client.SendAsync(data, data.Length, endPoint);
        }
    }
}
