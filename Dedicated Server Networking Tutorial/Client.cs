using System;
using System.Net.Sockets;

namespace Dedicated_Server_Networking_Tutorial
{
    public class Client
    {
        public static int DataBufferSize = 4096;

        public int Id;
        public TCP Tcp;

        public Client(int clientId)
        {
            Id = clientId;
            Tcp = new TCP(Id);
        }

        public class TCP
        {
            public TcpClient Socket;
            private readonly int _id;
            private NetworkStream _stream;
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
                _recieveBuffer = new byte[DataBufferSize];

                _stream.BeginRead(_recieveBuffer, 0, DataBufferSize, RecieveCallback, null);
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

                    _stream.BeginRead(_recieveBuffer, 0, DataBufferSize, RecieveCallback, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error recieving TCP data: {ex}");
                    // TODO disconnect
                }
            }
        }
    }
}