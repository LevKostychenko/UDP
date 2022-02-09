using System;
using System.Threading.Tasks;
using UDP.Client.Core;

namespace UDP.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine(
                "Welcome to the UDP chat app. To continue enter name: ");

            var userChatName = Console.ReadLine();

            var engine = new ChatEngine(
                userChatName);

            await Task.Run(() => engine.RunChat());
        }
    }
}
