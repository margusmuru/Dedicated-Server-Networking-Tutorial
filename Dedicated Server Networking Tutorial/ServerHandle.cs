using System;

namespace Dedicated_Server_Networking_Tutorial
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string userName = packet.ReadString();
            Console.WriteLine($"{Server.Clients[fromClient].Tcp.Socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
            if (fromClient != clientIdCheck)
            {
                Console.WriteLine($"Player \"{userName}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
            }
        }

        public static void UdpTestReceieved(int toClient, Packet packet)
        {
            string msg = packet.ReadString();
            Console.WriteLine($"Recieved packed via UDP. Contains message: {msg}");
        }
    }
}