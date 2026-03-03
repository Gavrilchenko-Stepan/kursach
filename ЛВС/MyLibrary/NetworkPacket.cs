using System;

namespace Messenger.Shared.Models
{
    [Serializable]
    public class NetworkPacket
    {
        public NetworkCommand Command { get; set; }
        public int SenderId { get; set; }
        public object Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
