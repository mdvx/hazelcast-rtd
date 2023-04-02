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

namespace HazelcastRtd
{
    [   // change this GUID for your version
        // This is the string that names RTD server.
        // Users will use it from Excel: =RTD("hazelcast",, ....)
        Guid("BC7560C1-2D7E-48F4-AFB1-57941110DF0B"),
        ProgId("hazelcast"),
        ComVisible(true)
    ]
    public class HazelcastRtdServer : IRtdServer
    {

        private IRtdUpdateEvent rtdCallback;

        private Dictionary<int, Topic> topics = new Dictionary<int, Topic>();
        private DataCache cache = new DataCache();

        private IHazelcastClient client;
        
        public HazelcastRtdServer()
        {
        }
        // Excel calls this. It's an entry point. It passes us a callback
        // structure which we save for later.
        int IRtdServer.ServerStart(IRtdUpdateEvent callback)
        {
            try
            {
                rtdCallback = callback;
                cache.DataUpdated += this.DataUpdated;

                //this.client = HazelcastClientFactory.StartNewClientAsync(new HazelcastOptionsBuilder()
                //            .WithConsoleLogger(LogLevel.Information)
                //            .Build()).Result;

                return 1;   // SUCCESS
            }
            catch (Exception e)
            {
                return -1;  // 0 or negative FAIL
            }
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
                return e.ToString();
            }

            return "ERROR: Need a string";
        }
        private object SubscribeHazelcast(int topicId, string host, string channel, string field)
        {
            //try
            //{
                if (String.IsNullOrEmpty(host))
                    host = "LOCALHOST";

                if (String.IsNullOrEmpty(channel))
                    return "#channel Required#";

            //    if (_subMgr.Subscribe(topicId, host, channel, field))
            //        return _subMgr.GetValue(topicId); // already subscribed 

            //    if (!_subscribers.TryGetValue(host, out Subscriber subscriber))
            //    {
            //        IHTopic<string> hzTopic = client.GetTopicAsync<string>("my-topic").Result;
            //        _subscribers[host] = subscriber = new Subscriber(hzTopic);  
            //    }

            //    Logger.Debug($"subscribing to {channel}");

            //    object value = subscriber.Subscribe(channel, (chan, message) =>
            //    {
            //        var rtdSubTopic = SubscriptionManager.FormatPath(host, chan);
            //        try
            //        {
            //            var str = message.ToString();
            //            _subMgr.Set(rtdSubTopic, str);

            //            if (str.StartsWith("{"))
            //            {
            //                var jo = JsonConvert.DeserializeObject<Dictionary<String, object>>(str);

            //                lock (_syncLock)
            //                {
            //                    foreach (string field_in in jo.Keys)
            //                    {
            //                        var rtdTopicString = SubscriptionManager.FormatPath(host, channel, field_in);
            //                        object val = jo[field_in];

            //                        _subMgr.Set(rtdTopicString, val);
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            _subMgr.Set(rtdSubTopic, ex.Message);
            //        }
            //    });
            //}
            //catch (Exception ex)
            //{
            //    _subMgr.Set(topicId, ex.Message);
            //}
            return cache.GetValue(topicId);
        }
        // Excel calls this when it wants to cancel subscription.
        void IRtdServer.DisconnectData(int topicId)
        {
            //var symbol = topics[topicId].Symbol;
            topics.Remove(topicId);
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
                data[1, index] = cache.GetValue(item.Value.Symbol);
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
