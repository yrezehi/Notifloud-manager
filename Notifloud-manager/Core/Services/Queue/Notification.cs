using Core.Services.Queue.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Queue
{
    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public int TTL { get; set; }
        public NotificationPriority Priority { get; set; }
    }
}
