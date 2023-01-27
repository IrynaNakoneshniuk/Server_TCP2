using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;

namespace Server_TCP2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ServerConnection.StartServer();
            while (true)
            {
                await Task.Run(() => ServerConnection.WriteMsgAsync());
                await Task.Run(() => ServerConnection.GetLogsOfConnection());
            }
        }
    }
}
    

