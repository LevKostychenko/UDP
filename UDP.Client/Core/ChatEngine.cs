using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UDP.Client.Extensions;
using UDP.Client.Services;

namespace UDP.Client.Core
{
    internal class ChatEngine
    {
        private readonly string _userName;
        private readonly int _localListeningPort;
        private readonly NetworkService _networkService;

        public ChatEngine(
            string userName)
        {
            _networkService = new NetworkService();
            _userName = userName;
            _localListeningPort = _networkService
                .GetFreeLocalPort();
        }

        public void RunChat()
        {
            var sendingTask = Task.Run(
                async () => await RunSendingClientAsync());

            var receivingTask = Task.Run(
                () => RunReceivingClient());

            Task.WaitAny(sendingTask, receivingTask);
        }

        private void RunReceivingClient()
        {
            var receiver = new UdpClient(
                _networkService.GetFreeLocalPort());
            receiver.JoinMulticastGroup(
                _networkService.GetMulticastAddress(), 
                20);

            IPEndPoint remoteIp = null;
            var localAddress = _networkService.GetLocalIp();

            try
            {
                while (true)
                {
                    var data = receiver.Receive(ref remoteIp);

                    if (remoteIp.Address.ToString()
                        .Equals(localAddress))
                    {
                        continue;
                    }
                        
                    string message = Encoding.Unicode.GetString(data);
                    Console.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
            }
        }

        private async Task RunSendingClientAsync()
        {
            var sender = new UdpClient();
            
            try
            {
                await sender.SendBroadcastMessageAsync(
                    $"{_userName} has joined the chat",
                    "SYSTEM");

                while (true)
                {
                    string message = Console.ReadLine();
                    await sender.SendMulticastMessageAsync(
                        message,
                        _userName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }
    }
}
