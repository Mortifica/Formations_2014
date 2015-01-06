using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TomShane.Neoforce.Controls;

namespace Formations.Connection
{
    public class ConnectionManager
    {
        // Connection info
        private const String SERVER_IP = "97.86.247.65";
        private const int SERVER_PORT = 15000;
        private const int PLAYER_PORT = 16000;

        // This
        private static ConnectionManager cm;

        // Server connection objects
        private TcpClient serverClient;
        private NetworkStream serverClientNS;
        private Task serverSenderThread;

        // Other player connection objects
        private TcpClient playerClient;
        private NetworkStream playerClientNS;

        private GameLobby gameLobby;
        private Chat chat;
        private GameBoard game;

        #region - Constructors

        private ConnectionManager()
        {
            if (gameLobby == null)
            {
                gameLobby = GameLobby.getInstance();
            }

            serverSenderThread = Task.Factory.StartNew(() => ServerConnection());
        }

        #endregion - Contructors

        #region - Public Methods

        // Call to getting the Connection Manager
        public static ConnectionManager getInstance()
        {
            if (cm == null)
            {
                cm = new ConnectionManager();
            }
            return cm;
        }
        public void setGame(GameBoard game)
        {
            this.game = game;
            this.chat = game.getChat();
        }
        public void sendChallengeRequect(ChallengeRequest request)
        {
            if (serverClient.Connected)
            {
                ConnectionMessage obj = Serialize(request);

                serverClientNS.Write(obj.Data, 0, obj.Data.Length);
                serverClientNS.Flush();

                if (request.IsAccepted)
                {
                    gameLobby.chatHistoryTextbox.Text += request.ToString() + " is sending Challenge\n";
                    var t = Task.Factory.StartNew(() => PlayerConnect());
                }

            }
        }

        // Method for sending a String, specifially the chat method.
        public void sendMessageServer(String message)
        {
            // Add additional logic to use this for playerClient
            if (serverClient.Connected)
            {
                message = "<" + gameLobby.person.Name + "> " + message;

                ConnectionMessage obj = Serialize(message);

                serverClientNS.Write(obj.Data, 0, obj.Data.Length);
                serverClientNS.Flush();
            }
        }
        // Method for sending a String, specifially the chat method.
        public void sendMessagePlayer(String message)
        {
            // Add additional logic to use this for playerClient
            if (playerClient.Connected)
            {
                message = "<" + gameLobby.person.Name + "> " + message;
                chat.chatHistoryTextbox.Text += message + "\n";
                ConnectionMessage obj = Serialize(message);

                playerClientNS.Write(obj.Data, 0, obj.Data.Length);
                playerClientNS.Flush();
            }
        }
        // Method for sending a String, specifially the chat method.
        public void sendSerialClassPlayer(SerialClass serialClass)
        {
            // Add additional logic to use this for playerClient
            /*if (playerClient.Connected)
            {
                ConnectionMessage obj = Serialize(serialClass);

                playerClientNS.Write(obj.Data, 0, obj.Data.Length);
                playerClientNS.Flush();

                // Put this after it was actually sent
                chat.chatHistoryTextbox.Text += serialClass + " serial class sent. Size " + obj.Data.Length + "\n";
                //Deserialize(obj);
            }*/
        }
        public void sendPerson(Person person)
        {
            if (serverClient.Connected)
            {
                ConnectionMessage obj = Serialize(person);

                serverClientNS.Write(obj.Data, 0, obj.Data.Length);
                serverClientNS.Flush();
            }
        }

        // Close the damn connection, the .NET framework seems to be taking care of it, so... that's good.
        public void closeConnection()
        {
            playerClientNS.Close();
            playerClient.Close();
            serverClientNS.Close();
            serverClient.Close();
        }

        // Returns if the playerClient is connected
        public Boolean isConnectedToPlayer()
        {
            if (playerClient == null)
                return false;
            return playerClient.Connected;
        }

        // Returns if the serverClient is connected
        public Boolean isConnectedToServer()
        {
            if (serverClient == null)
                return false;
            return serverClient.Connected;
        }

        #endregion - Public Methods

        #region - Private Methods

        // The constant listen fuction, works for server and otherPlayer
        private void listen(TcpClient client)
        {
            // Something is available
            if (client.Available != 0)
            {
                byte[] buffer = new byte[client.ReceiveBufferSize];

                //---read incoming stream--- Will place the data into the buffer
                int bytesRead = client.GetStream().Read(buffer, 0, client.ReceiveBufferSize);

               // if (isConnectedToPlayer())
                 //   chat.chatHistoryTextbox.Text += bytesRead + " bytes read.\n"; //---write back the text to the client---

                ConnectionMessage message = new ConnectionMessage { Data = buffer };

                // Deserialize then figure out what the object we got was.
                object obj = Deserialize(message);
                if (obj is String)
                {
                    if (game != null)
                    {
                        chat.chatHistoryTextbox.Text = (obj as String) + "\n" + chat.chatHistoryTextbox.Text;//set chat history here chat.
                    }
                    else
                    {
                        gameLobby.chatHistoryTextbox.Text = (obj as String) + "\n" + gameLobby.chatHistoryTextbox.Text; //---write back the text to the client---
                    }
                }
                else if (obj is Person)
                {
                    //gameLobby.chatHistoryTextbox.Text += (obj as Person) + "\n"; //---write back the text to the client---
                    gameLobby.updatePlayersList((Person)obj);
                }
                else if (obj is ChallengeRequest)
                {
                    var cr = obj as ChallengeRequest;

                    if (cr.IsAccepted && gameLobby.person.Equals(cr.Sender))
                    {
                        gameLobby.CurrentRequest = cr;
                        // Start player host
                        gameLobby.chatHistoryTextbox.Text += cr.ToString() + " is Accepted\n"; //---write back the text to the client---
                        var t = Task.Factory.StartNew(() => PlayerListener());
                    }
                    else
                    {
                        if (!gameLobby.person.Equals(cr.Sender) && gameLobby.person.Equals(cr.Reciever) && !cr.IsAccepted)
                        {
                            gameLobby.chatHistoryTextbox.Text += cr.ToString() + " is Recieving\n"; //---write back the text to the client---
                            gameLobby.AcceptChallengeWindowOpen(cr);
                        }
                    }
                }
                else if (obj is SerialClass)
                {
                    game.setSerialClass((SerialClass)obj);
                }
            }
        }

        // This is to send off to a player to establish the connection "Join Game"
        private void PlayerConnect()
        {
            Thread.Sleep(3000);

            try
            {
                // This is the connection to the player, need it to attempt a few times.
                playerClient = new TcpClient(gameLobby.CurrentRequest.Sender.ipAddress, PLAYER_PORT);
            }
            catch (SocketException)
            {
                gameLobby.chatHistoryTextbox.Text += "Unable to connect.\n";
                return;
            }
            // Set the NS stream reference
            playerClientNS = playerClient.GetStream();

            // Place the listen on its own thread
            var t = Task.Factory.StartNew(() => PlayerListener());
        }

        // Listener method will be placed on its own thread, uses the client TcpClient
        private void PlayerListener()
        {
            // To start hosting
            if (playerClient == null)
            {
                IPAddress localAdd = IPAddress.Any;
                TcpListener listener = new TcpListener(localAdd, PLAYER_PORT);
                listener.Start();

                // Accept the connection
                playerClient = listener.AcceptTcpClient();

                // Get the data stream from the player
                playerClientNS = playerClient.GetStream();
            }

            try
            {
                chat.chatHistoryTextbox.Text += "Player connection established.\n";

                while (playerClient.Connected)
                {
                    // This will make the server listen
                    listen(playerClient);
                    Thread.Sleep(10);
                }
            }
            catch (Exception e)
            {
                chat.chatHistoryTextbox.Text += "Exception happened with PlayerClient. Connection Ended.\n" + e + "\n";
                playerClient = null;
                return;
            }
        }

        // Build's the sender datastream, uses the server TcpClient
        private void ServerConnection()
        {
            try
            {
                // This is the connection to the server
                serverClient = new TcpClient(SERVER_IP, SERVER_PORT);
            }
            catch (SocketException)
            {
                gameLobby.chatHistoryTextbox.Text += "Unable to connect.\n";
                return;
            }

            // Set the NS stream reference
            serverClientNS = serverClient.GetStream();

            // Create a byte array to store the incoming IP address
            byte[] buffer = new byte[serverClient.ReceiveBufferSize];

            // Read the data that's come in
            int bytesRead = serverClientNS.Read(buffer, 0, serverClient.ReceiveBufferSize);
            // Getting only the IP
            char[] chars = new char[buffer.Length / sizeof(char)];
            System.Buffer.BlockCopy(buffer, 0, chars, 0, buffer.Length);
            String ip = "";
            for (int i = 0; i < 16; i++)
            {
                System.Console.WriteLine(chars[i]);
                if (chars[i] == '\0') { break; }
                ip += chars[i];
            }
            gameLobby.person.ipAddress = ip;
            gameLobby.chatHistoryTextbox.Text += ip + "\n";

            gameLobby.chatHistoryTextbox.Text += "Connection to server established.\n";

            ServerListener();
        }

        // Listener method will be placed on its own thread, uses the client TcpClient
        private void ServerListener()
        {
            try
            {
                while (serverClient.Connected)
                {
                    // This will make the server listen
                    listen(serverClient);
                    Thread.Sleep(10);
                }
            }
            catch (Exception)
            {
                gameLobby.chatHistoryTextbox.Text += "Exception happened with ServerClient. Connection Ended.\n";
                serverClient = null;
                return;
            }
        }

        // Encode message for use
        private ConnectionMessage Serialize(object someObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(memoryStream, someObject);
                return new ConnectionMessage { Data = memoryStream.ToArray() };
            }
        }

        // Decode object for use
        private object Deserialize(ConnectionMessage message)
        {
            using (var memoryStream = new MemoryStream(message.Data))
            {
                return (new BinaryFormatter()).Deserialize(memoryStream);
            }
        }

        #endregion - Private Methods

    }
}
