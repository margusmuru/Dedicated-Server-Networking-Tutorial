using System.Threading;

namespace Dedicated_Server_Networking_Tutorial
{
    public class GameLogic
    {
        public static void Update()
        {
            ThreadManager.UpdateMain();
        }
    }
}