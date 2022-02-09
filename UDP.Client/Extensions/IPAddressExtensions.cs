using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace UDP.Client.Extensions
{
    internal static class IPAddressExtensions
    {
        public static bool IsLocalAddress(
            this IPAddress address)
            => GetLocalIps()
                .Select(x => x.ToString())
                .Contains(address.ToString());

        private static IEnumerable<IPAddress> GetLocalIps()
        {
            var localIp = string.Empty;
            var host = Dns.GetHostEntry(
                Dns.GetHostName());

            return host
                .AddressList
                .Where(
                    x => x.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
