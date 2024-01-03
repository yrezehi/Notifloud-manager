using Core.Models;
using Core.Repositories.Abstracts.Interfaces;
using Core.Services.Abstract;

namespace Core.Services
{
    public class SubscriptionsService : ServiceBase<Subscription>
    {
        public readonly string PublicKey { get; set; }

        public SubscriptionsService(IUnitOfWork unitOfWork) : base(unitOfWork) =>
            PublicKey = "X83GLEFW933";
           
        
    }
}
