using System;

namespace TCP_Bog_server
{
    class Program
    {
        static void Main(string[] args)
        {
            

            Server testServer = new Server();
            testServer.Start();

            Console.ReadLine();
        }
    }
}
