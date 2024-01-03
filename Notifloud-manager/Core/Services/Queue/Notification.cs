using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Queue
{
    public class Notification
    {
        public int TTL { get; set; }
        public NotificationPriority Priority { get; set; }
    }
}
