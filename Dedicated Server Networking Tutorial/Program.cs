using System;

namespace Dedicated_Server_Networking_Tutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Game Server";
            Console.WriteLine("Initializing server...");
            
            Server.Start(4, 26950);
            
            Console.ReadKey();
        }
    }
}