using System;
using System.Numerics;

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
            Server.Clients[fromClient].SendIntoGame(userName);
        }

        public static void PlayerMovement(int fromClient, Packet packet)
        {
            bool[] inputs = new bool[packet.ReadInt()];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = packet.ReadBool();
            }

            Quaternion rotation = packet.ReadQuaternion();

            Server.Clients[fromClient].Player.SetInputs(inputs, rotation);
        }
        
    }
}