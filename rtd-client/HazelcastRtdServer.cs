using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using HazelcastRTD;
using Hazelcast.DistributedObjects;
using Hazelcast;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace HazelcastRTD
{
    [   // change this GUID for your version
        // This is the string that names RTD server.
        // Users will use it from Excel: =RTD("hazelcast",, ....)
        ProgId("hazelcast.rtd"),
        ComVisible(true)
    ]
    [Guid("94872AEA-8BBA-4AEE-9965-D9D9878F3834")]
    public class HazelcastRtdServer : IRtdServer
    {
        private IRtdUpdateEvent rtdCallback;

        private Dictionary<string, int> topics = new Dictionary<string, int>();

        private DataCache cache = new DataCache();
        private Thread worker;
        private IHazelcastClient client;


        public HazelcastRtdServer()
        {
        }
        public static void Main(string[] args)
        {
        }
        // Excel calls this. It's an entry point. It passes us a callback
        // structure which we save for later.
        int IRtdServer.ServerStart(IRtdUpdateEvent callback)
        {
            Debug.WriteLine("ServerStart");

                rtdCallback = callback;
                cache.DataUpdated += this.DataUpdated;

                Thread worker = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        //client = HazelcastClientFactory.StartNewClientAsync(new HazelcastOptionsBuilder()
                        //            //.WithConsoleLogger(LogLevel.Information)
                        //            .Build()).Result;

                        //client.SubscribeAsync((e) => { Debug.WriteLine("ServerStart: SubscribeAsync", e); }).Wait();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("ERR: {}", ex.Message);
                        //return -1;  // 0 or negative FAIL
                    }

                }));
                worker.Start();
                worker.Join();

                return 1;   // SUCCESS
        }

        // Excel calls this when it wants to make a new topic subscription.
        // topicId becomes the key representing the subscription.
        // String array contains any aux data user provides to RTD macro.
        dynamic IRtdServer.ConnectData(int topicId, ref Array strings, ref bool newValues)
        {
            try
            {
                newValues = true;

                if (strings.Length == 1)
                {
                    return strings.GetValue(0);
                }
                else if (strings.Length >= 2)
                {
                    // Crappy COM-style arrays...
                    string host = strings.GetValue(0).ToString().ToUpperInvariant();
                    string channel = strings.GetValue(1).ToString();
                    string field = strings.Length > 2 ? strings.GetValue(2).ToString() : "";

                    return SubscribeHazelcast(topicId, host, channel, field);
                }
            } catch(Exception e)
            {
                return "#" + e.ToString();
            }

            return "ERROR: Need a string";
        }
        private object SubscribeHazelcast(int topicId, string host, string channel, string field)
        {
            if (String.IsNullOrEmpty(host))
                host = "LOCALHOST";

            if (String.IsNullOrEmpty(channel))
                return "#channel Required#"; 

            object result;
            if (cache.TryGetValue(topicId, out result))
                return result;


            var hzTopic = channel + "|" + field;

            Thread worker = new Thread(new ThreadStart(() =>
            {
                while (client == null)
                    ;

                IHTopic<string> topic = client.GetTopicAsync<string>(hzTopic).Result;
                topics.Add(hzTopic, topicId);

                topic.SubscribeAsync((evt) => {
                    Debug.Print("{}", evt);
                    cache.AddToCache(hzTopic, topicId);
                });
            }));
            worker.Start();

            return cache.GetValue(topicId);
        }
        // Excel calls this when it wants to cancel subscription.
        void IRtdServer.DisconnectData(int topicId)
        {
            //var symbol = topics[topicId].Symbol;
            //topics.Remove(topicId);
        }
        // Excel calls this every once in a while.
        int IRtdServer.Heartbeat()
        {
            return 1;
        }

        // Excel calls this to get changed values. 
        Array IRtdServer.RefreshData(ref int topicCount)
        {
            object[,] data = new object[2, topics.Count];
            int index = 0;
            foreach (var item in topics)
            {
                data[0, index] = item.Key;
                data[1, index] = cache.GetValue(0);
                index++;
            }
            topicCount = topics.Count;        //update Excel side topic count
            return data;
        }
        private void DataUpdated(object sender, object arg)
        {
            if (rtdCallback != null)        //rtdCallback is passed from Excel
                rtdCallback.UpdateNotify(); //here, notify Excel we have data updated
        }
        // Excel calls this when it wants to shut down RTD server.
        void IRtdServer.ServerTerminate()
        {
            rtdCallback = null;
            
            if (client != null)
                client.DestroyAsync(null);

            client = null;
            topics.Clear();
        }

        // Helper function which checks if new data is available and,
        // if so, notifies Excel about it.
        private void TimerElapsed(object sender, EventArgs e)
        {
            //if (_subMgr.IsDirty)
            //    _subMgr.Set(LAST_RTD, DateTime.Now.ToLocalTime());

            //_subMgr.Set(CLOCK, DateTime.Now.ToLocalTime());
        }
    }

}
