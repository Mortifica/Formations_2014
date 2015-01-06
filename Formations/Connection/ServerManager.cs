using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Formations.Connection
{
    class ServerManager
    {
        private const int PORT = 15000;
        private TcpListener listener;
        private List<TcpClient> listOfConnections;


        public ServerManager()
        {
            listOfConnections = new List<TcpClient>();

            //---listen at the specified IP and port no.---
            IPAddress localAdd = IPAddress.Any;
            listener = new TcpListener(localAdd, PORT);
        }

        public void Run()
        {
            Console.WriteLine("Server is Running...");
            MessageBox.Show("Server is Running...");

            // Begin Hosting
            listener.Start();

            while (true)
            {
                listen(listener, listOfConnections);
                Thread.Sleep(10); // Prevent the thread from locking up
            }
        }

        // The constant listening function
        private void listen(TcpListener listener, List<TcpClient> listOfConnections)
        {
            // Check if there is a connection pending.
            if (!listener.Pending())
            {
                return;
            }

            // Accept the connection
            TcpClient client = listener.AcceptTcpClient();

            //---get the incoming data through a network stream---
            NetworkStream ns = client.GetStream();

            // Write back the IP to the user
            var str = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            ns.Write(bytes, 0, bytes.Length);

            // Add the connection to the list of connection.
            listOfConnections.Add(client);
            Console.Out.WriteLine("Connection from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

            // Start up a thread with the connection for sending and listening
            var t = Task.Factory.StartNew(() => ClientThread(client, listOfConnections));
        }


        private void ClientThread(TcpClient client, List<TcpClient> listOfConnections)
        {
            Boolean connection = true;

            while (connection)
            {
                try
                {
                    connection = Sender(client, listOfConnections);
                }
                catch (Exception e)
                {
                    //Console.Out.WriteLine("Exception: " + e);
                    listOfConnections.Remove(client);
                    Console.Out.WriteLine("Disconnect from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                    return; // Kill this thread.
                }

                // Prevent the thread from locking up.
                Thread.Sleep(20);
            }
        }


        private Boolean Sender(TcpClient client, List<TcpClient> listOfConnections)
        {
            // Detect if client disconnected
            if (client.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] buff = new byte[1];
                if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
                {
                    // Client disconnected
                    listOfConnections.Remove(client);
                    Console.Out.WriteLine("Disconnect from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                    return false;
                }
            }

            // Check if they have an data written
            if (client.GetStream().DataAvailable)
            {
                // Break this into method to send data to other TcpClients
                var ns = client.GetStream();

                byte[] buffer = new byte[client.ReceiveBufferSize];

                //---read incoming stream--- Will place the data into the buffer
                int bytesRead = ns.Read(buffer, 0, client.ReceiveBufferSize);

                // Forward it to everyone
                foreach (TcpClient clients in listOfConnections)
                {
                    var ns2 = clients.GetStream();
                    ns2.Write(buffer, 0, bytesRead);
                    ns2.Flush();
                }
            }

            return true;
        }

    }
}
