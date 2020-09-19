using System;
using System.Net;
using System.Net.Sockets;

namespace Dedicated_Server_Networking_Tutorial
{
    public class Client
    {
        public static int DataBufferSize = 4096;

        public int Id;
        public TCP Tcp;
        public UDP Udp;

        public Client(int clientId)
        {
            Id = clientId;
            Tcp = new TCP(Id);
            Udp = new UDP(Id);
        }

        public class TCP
        {
            public TcpClient Socket;
            private readonly int _id;
            private NetworkStream _stream;
            private Packet _receivedData;
            private byte[] _recieveBuffer;

            public TCP(int id)
            {
                _id = id;
            }

            public void Connect(TcpClient socket)
            {
                Socket = socket;
                socket.ReceiveBufferSize = DataBufferSize;
                socket.SendBufferSize = DataBufferSize;
                _stream = socket.GetStream();
                _receivedData = new Packet();
                _recieveBuffer = new byte[DataBufferSize];

                _stream.BeginRead(_recieveBuffer, 0, DataBufferSize, RecieveCallback, null);

                ServerSend.Welcome(_id, "Welcome to the server!");
            }

            private void RecieveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLength = _stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        // TODO disconnect
                        return;
                    }
                    byte[] data = new byte[byteLength];
                    Array.Copy(_recieveBuffer, data, byteLength);
                    _receivedData.Reset(HandleData(data));
                    _stream.BeginRead(_recieveBuffer, 0, DataBufferSize, RecieveCallback, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error recieving TCP data: {ex}");
                    // TODO disconnect
                }
            }
            
            private bool HandleData(byte[] data)
            {
                int packetLength = 0;
                _receivedData.SetBytes(data);
                if (_receivedData.UnreadLength() >= 4)
                {
                    packetLength = _receivedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (packetLength > 0 && packetLength <= _receivedData.UnreadLength())
                {
                    byte[] packetBytes = _receivedData.ReadBytes(packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet packet = new Packet(packetBytes))
                        {
                            int packetId = packet.ReadInt();
                            Server.PacketHandlers[packetId](_id, packet);
                        }
                    });

                    packetLength = 0;
                    if (_receivedData.UnreadLength() >= 4)
                    {
                        packetLength = _receivedData.ReadInt();
                        if (packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (packetLength <= 1)
                {
                    return true;
                }

                return false;
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (Socket != null)
                    {
                        _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error sending data to player {_id} via TCP: {e}");
                }
            }
        }

        public class UDP
        {
            public IPEndPoint EndPoint;
            private int _id;

            public UDP(int id)
            {
                _id = id;
            }

            public void Connect(IPEndPoint endPoint)
            {
                EndPoint = endPoint;
                ServerSend.UdpTest(_id);
            }

            public void SendData(Packet packet)
            {
                Server.SendUdpData(EndPoint, packet);
            }

            public void HandleData(Packet packetData)
            {
                int packetLength = packetData.ReadInt();
                byte[] packetBytes = packetData.ReadBytes(packetLength);
                
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Server.PacketHandlers[packetId](_id, packet);
                    }
                });
            }
        }
    }
}