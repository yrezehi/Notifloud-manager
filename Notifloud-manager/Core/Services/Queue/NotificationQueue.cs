using System.Collections.Concurrent;

namespace Core.Services.Queue
{
    public class NotificationQueue
    {
        private readonly ConcurrentQueue<Notification> Notifications;
        private readonly SemaphoreSlim Signal = new SemaphoreSlim(0);

        public NotificationQueue() =>
            (Notifications, Signal) = (new ConcurrentQueue<Notification>(), new SemaphoreSlim(0));
       
        public void Enqueue(Notification notification)
        {
            Notifications.Enqueue(notification);
            Signal.Release();
        }

        public async Task<Notification> Dequeue(CancellationToken cancellationToken)
        {
            await Signal.WaitAsync(cancellationToken);

            Notifications.TryDequeue(out Notification? notification);

            if(notification == null)
            {
                throw new MemberAccessException("No notification left!");
            }

            return notification;
        }
    }
}
