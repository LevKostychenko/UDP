using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UDP.Client.Commands;
using UDP.Client.Extensions;
using UDP.Client.Services;

namespace UDP.Client.Core
{
    internal class ChatEngine
    {
        private readonly string _userName;
        private readonly NetworkService _networkService;
        private UdpClient receiver;
        private UdpClient sender;

        private IList<IPAddress> ignoredAddresses;

        public ChatEngine(
            string userName)
        {
            _networkService = new NetworkService();
            _userName = userName;

            ignoredAddresses = new List<IPAddress>();
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
            receiver = new UdpClient(8001);
            receiver.JoinMulticastGroup(
                _networkService.GetMulticastAddress(),
                20);
            receiver.MulticastLoopback = false;

            IPEndPoint remoteIp = null;

            try
            {
                while (true)
                {
                    var data = receiver.Receive(ref remoteIp);

                    if (ignoredAddresses.Any(
                        x => remoteIp.ToString()
                            .Contains(x.ToString())))
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
            sender = new UdpClient();

            try
            {
                await sender.SendBroadcastMessageAsync(
                    $"{_userName} has joined the chat",
                    "SYSTEM");

                while (true)
                {
                    string message = Console.ReadLine();

                    if (IsCommand(message))
                    {
                        ExecuteCommand(message);
                        continue;
                    }

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

        private void ExecuteCommand(
            string message)
        {
            var (command, arguments) = ParseCommand(message);

            switch (command.ToUpper())
            {
                case ConsoleCommands.MulticastExit:
                    {
                        receiver.DropMulticastGroup(
                            _networkService.GetMulticastAddress());
                        Console.WriteLine("Exit from multicast.");
                        break;
                    }
                case ConsoleCommands.Ignore:
                    {
                        AddIgnoredAddresses(arguments);
                        Console.WriteLine($"{string.Join(',', arguments)} are now ignoring.");
                        break;
                    }
                case ConsoleCommands.MulticastJoin:
                    {
                        receiver.JoinMulticastGroup(
                            _networkService.GetMulticastAddress(),
                            20);
                        Console.WriteLine($"Join to the multicast.");
                        receiver.MulticastLoopback = false;
                        break;
                    }
                case ConsoleCommands.ShowParticipants:
                    {
                        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                        {
                            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                            {
                                if (!ip.IsDnsEligible)
                                {
                                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                    {
                                        // All IP Address in the LAN
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }

        private bool IsCommand(
            string message)
            => message.StartsWith(@"/");

        private (string, IEnumerable<string>) ParseCommand(
            string message)
        {
            var command = message.Split(" ")[0][1..];
            var arguments = message.Split(" ")
                .Skip(1);

            return (command, arguments);
        }

        private void AddIgnoredAddresses(
            IEnumerable<string> commandArguments)
        {
            foreach (var address in commandArguments)
            {
                IPAddress parsedAddress;

                if (IPAddress.TryParse(
                    address.Trim(), out parsedAddress))
                {
                    ignoredAddresses.Add(parsedAddress);
                }
            }
        }
    }
}
