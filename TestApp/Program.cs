using System;
using System.Threading;
using System.Windows.Threading;
using Hazelcast;
using Hazelcast.DistributedObjects;


namespace TestApp
{
    class Program
    {
        string[] args;
        static CancellationTokenSource cts = new CancellationTokenSource();
        

        public static void Main(string[] args)
        {
            Console.WriteLine("Main");

            IHazelcastClient client = HazelcastClientFactory.StartNewClientAsync(new HazelcastOptionsBuilder().Build()).Result;
            IHTopic<string> topic = client.GetTopicAsync<string>("my-topic").Result;

            for(int i = 0; i < 100 && !cts.IsCancellationRequested; ++i)
            {
                Console.WriteLine("RAW_"+i);
                topic.PublishAsync("RAW_" + i);
                Thread.Sleep(5000);
            }
            // Start up a Windows message pump and spin forever.
            Dispatcher.Run();
        }

    }
}
