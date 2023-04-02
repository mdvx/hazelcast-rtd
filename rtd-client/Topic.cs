using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HazelcastRTD
{
    internal class Topic
    {
        public int TopicId { get; set; }         //the value passed from Excel
        public string Symbol { get; set; }       //the value passed from Excel
        public string TopicType { get; set; }    //the value passed from Excel
    }
}
