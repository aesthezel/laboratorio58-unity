using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SocketClient.Runtime.Models
{
    public class Instance
    {
        public int InstanceId { get; set; }
        public List<PlayerPosition> Players { get; set; }
    }
}