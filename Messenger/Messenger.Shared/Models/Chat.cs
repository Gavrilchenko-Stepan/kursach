using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Shared.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ChatType Type { get; set; }
        public int? DepartmentId { get; set; }
        public int? OtherDepartmentId { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public List<User> Participants { get; set; } = new List<User>();
    }
}
