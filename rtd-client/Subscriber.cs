using Hazelcast.DistributedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HazelcastRTD
{
    internal class Subscriber
    {
        IHTopic<string> hzTopic;
        Action<string,string> callback;

        internal Subscriber(IHTopic<string> topic)
        {
            this.hzTopic = topic;
            hzTopic.SubscribeAsync(e => {
                callback(hzTopic.Name, e.ToString());
            });
        }
        internal object Subscribe(string channel, Action<string,string> callback)
        {
            this.callback = callback;
            return channel; // cached
        }
    }
}
