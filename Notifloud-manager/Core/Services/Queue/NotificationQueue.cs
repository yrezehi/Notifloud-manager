using System.Collections.Concurrent;

namespace Core.Services.Queue
{
    public class NotificationQueue
    {
        private readonly ConcurrentQueue<string> Notifications;

        public NotificationQueue() =>
            (Notifications) = (new ConcurrentQueue<string>());
       
        public void Enqueue()
        {

        }

        public void Dequeue()
        {

        }
    }
}
