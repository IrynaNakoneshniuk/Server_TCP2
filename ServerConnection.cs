using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Security;

namespace Server_TCP2
{
    public static class ServerConnection
    {
        public static TcpListener server ;
        private static IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        private static TcpConnectionInformation[] tcpConnections =ipProperties.GetActiveTcpConnections();
        public static async Task<string> ReadMsgAsync(TcpClient clt)
        {
            using (NetworkStream ns = clt.GetStream())
            {
                using (StreamReader sr = new StreamReader(ns))
                {
                    return await sr.ReadToEndAsync();
                }                
            }
        }
       
        public static void StartServer()
        {
            server = new TcpListener(IPAddress.Any, 8080);
            server.Start();
        }
        public static async Task WriteMsgAsync()
        {
            int countFraze = 0;
            TcpClient clt = await server.AcceptTcpClientAsync();
            string res = await ReadMsgAsync(clt);
            string[] arrRes = new string[2];
            arrRes = res.Split(' ');
            if (Authentification.Login.Select(i => i == arrRes[0]).First() != null &&
                Authentification.Password.Select(i => i == arrRes[1]).First() != null)
            {
                using (NetworkStream ns = clt.GetStream())
                {
                    using (StreamWriter sr = new StreamWriter(ns))
                    {
                        if (tcpConnections.Length < 5)
                        {
                            while (countFraze < 4)
                            {
                                await sr.WriteAsync(Fraze.FrazeArray[countFraze]);
                                countFraze++;
                            }
                        }
                        else
                        {
                            await sr.WriteAsync("Request is failed, overlimit of requests");
                            clt.Close();
                        }
                    }
                }
            }
            else
            {
                await WriteErrAuthent(clt);
            }
        }

        public static async Task WriteErrAuthent(TcpClient clt)
        {
            using (NetworkStream ns = clt.GetStream())
            {
                using (StreamWriter sr = new StreamWriter(ns))
                {
                    await sr.WriteAsync("Request is failed, uncorrect password or login!");
                    clt.Close();
                }
            }
        }
        public static async Task GetLogsOfConnection()
        {
            foreach (TcpConnectionInformation info in tcpConnections)
            {
                Console.WriteLine($"\n { info.RemoteEndPoint.Address}, {info.RemoteEndPoint.Port},{info.State.ToString()}");
            }
        }
        public static void ServerStop()
        {
            server.Stop();
        }
    }
}
