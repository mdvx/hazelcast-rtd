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
        CancellationTokenSource cts = new CancellationTokenSource();
        

        [STAThread]
        public static void Main(string[] args)
        {
            var me = new Program(args);
            //IRtdUpdateEvent me2 = me;
            //me2.HeartbeatInterval = 15;  // is this seconds or milliseconds?
            me.Run();
        }


        public Program(string[] args)
        {
            this.args = args;
        }

        void Run()
        {
            IHazelcastClient client = HazelcastClientFactory.StartNewClientAsync(new HazelcastOptionsBuilder().Build()).Result;
            IHTopic<string> topic = client.GetTopicAsync<string>("my-topic").Result;

            for(int i = 0; i < 100 && !cts.IsCancellationRequested; ++i)
            {
                topic.PublishAsync("RAW_" + i);
            }
            // Start up a Windows message pump and spin forever.
            Dispatcher.Run();
        }

    }
}
