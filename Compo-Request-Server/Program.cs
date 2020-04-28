using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Compo_Request_Server.Network;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;

namespace Compo_Request_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var Server = new ServerBase();

            try
            {
                var ServerThread = new Thread(new ThreadStart(Server.Listen));
                ServerThread.Start();
            }
            catch (Exception ex)
            {
                ServerBase.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}
