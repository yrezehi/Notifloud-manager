

using Core.Models;
using Core.Repositories.Abstracts.Interfaces;
using Core.Services.Abstract;

namespace Core.Services
{
    public class SubscriptionsService : ServiceBase<Subscription>
    {
        public SubscriptionsService(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
