using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HazelcastRTD
{
    internal class DataCache
    {
        private Dictionary<string, object> _cache = new Dictionary<string, object>();
        public event EventHandler<object> DataUpdated;

        public void AddToCache(string key, object value)
        {
            _cache.Add(key, value);
            DataUpdated(this, key);
        }
        public object GetValue(int topicId)
        {
            return topicId;
        }
        public object GetValue(String key)
        {
            object value;
            if (_cache.TryGetValue(key,out value))
                return value;

            return null;    
        }

        internal bool TryGetValue(int topicId, out object result)
        {
            return _cache.TryGetValue(topicId.ToString(), out result);
        }
    }
}
