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
        private readonly NetworkService _networkService;

        public ChatEngine(
            string userName)
        {
            _networkService = new NetworkService();
            _userName = userName;
        }

        public void RunChat()
        {
            var receivingTask = Task.Run(
                () => RunReceivingClient());
            var sendingTask = Task.Run(
                async () => await RunSendingClientAsync());

            Task.WaitAny(sendingTask, receivingTask);
        }

        private void RunReceivingClient()
        {
            var receiver = new UdpClient(8001);
            receiver.JoinMulticastGroup(
                _networkService.GetMulticastAddress(), 
                20);

            IPEndPoint remoteIp = null;

            try
            {
                while (true)
                {
                    var data = receiver.Receive(ref remoteIp);

                    if (remoteIp.Address.IsLocalAddress())
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
