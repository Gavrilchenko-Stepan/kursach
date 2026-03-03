using System.Collections.Generic;

namespace Messenger.Shared.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public int OnlineCount { get; set; }
    }
}
