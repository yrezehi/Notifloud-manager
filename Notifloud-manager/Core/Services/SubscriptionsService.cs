using Core.Models;
using Core.Repositories.Abstracts.Interfaces;
using Core.Services.Abstract;
using Core.Services.Queue;

namespace Core.Services
{
    public class SubscriptionsService : ServiceBase<Subscription>
    {
        public readonly string PublicKey { get; set; }
        public readonly NotificationQueue Notifications { get; set; }

        public SubscriptionsService(IUnitOfWork unitOfWork, NotificationQueue notifications) : base(unitOfWork) =>
            (Notifications, PublicKey) = (notifications, "X83GLEFW933");
           
        public void Enqueue(Notification notification) =>
            Notifications.Enqueue(notification);
    }
}
