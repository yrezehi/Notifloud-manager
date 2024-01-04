using Core.Models;
using Core.Services;
using Core.Services.Queue;
using Microsoft.AspNetCore.Mvc;
using Notifloud_manager.HTML;

namespace Notifloud_manager.Controllers
{
    public static class ControllerRegistry
    {
        public static void RegisterControllers(this WebApplication application)
        {
            application.RegisterUI();
        }

        private static void RegisterUI(this WebApplication application) {            
            application.Index();
            application.Assets();

            /*application.Subscribe();
            application.Subscriptions();

            application.Notification();*/
        }

        private static void Index(this WebApplication application) =>
            application.MapGet("/", () => Results.Extensions.Serve("index.html"));

        private static void Assets(this WebApplication application) =>
            application.MapGet("/Assets/{*asset}", (string asset) => Results.Extensions.Serve("Assets\\" + asset));

        private static void Subscribe(this WebApplication application) =>
            application.MapPost("/Subscriptions", async ([FromBody] Subscription subscription, SubscriptionsService service) => await service.Create(subscription));

        private static void Subscriptions(this WebApplication application) =>
           application.MapGet("/Subscriptions", async (SubscriptionsService service) => await service.GetAll());

        private static void Notification(this WebApplication application) =>
            application.MapPost("/Notifications", ([FromBody] Notification notification, SubscriptionsService service) => service.Enqueue(notification));
    }
}
