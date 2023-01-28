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
        public static async Task<string[]> ReadMsgAsync(NetworkStream ns)
        {
            byte[] bufRead = new byte[1024];
            string[] res = null;
            await ns.ReadAsync(bufRead);
            res = Encoding.Unicode.GetString(bufRead).Split(' ');
            return res;
        }
       
        public static void StartServer()
        {
            server = new TcpListener(IPAddress.Any, 8080);
            server.Start();
        }
        public static async Task WriteMsgAsync()
        {
            string res = null;
            int countFraze = 0;
            string[] arrRes = new string[2];
            TcpClient clt = await server.AcceptTcpClientAsync();
            NetworkStream ns = clt.GetStream();
            try
            {
                arrRes = await ReadMsgAsync(ns);
                
                if (Authentification.Login.Select(i => i == arrRes[0])!=null
                    && Authentification.Password.Select(i => i == arrRes[1])!=null)
                {
                    if (tcpConnections.Length < 20)
                    {
                        while (countFraze < 4)
                        {
                            byte[] buf = new byte[1024];
                            buf = Encoding.Unicode.GetBytes(Fraze.FrazeArray[countFraze]);
                            await ns.WriteAsync(buf);
                            countFraze++;
                        }
                    }
                    else
                    {
                        await ns.WriteAsync(Encoding.Unicode.GetBytes("Request is failed, overlimit of requests"));
                        clt.Close();
                    }
                }
                else
                {
                    await WriteErrAuthent(ns, clt);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ns.Close();
            }
        }

        public static async Task WriteErrAuthent(NetworkStream ns, TcpClient clt)
        {
            try
            {
                byte[] bufWrite = Encoding.Unicode.GetBytes("Request is failed, uncorrect password or login!");
                await ns.WriteAsync(bufWrite);
                clt.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
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
