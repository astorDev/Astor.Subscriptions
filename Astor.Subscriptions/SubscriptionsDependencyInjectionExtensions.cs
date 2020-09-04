using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Subscriptions
{
    public static class SubscriptionsDependencyInjectionExtensions
    {
        public static void AddSubscriptions(this IServiceCollection serviceCollection, Assembly assembly)
        {
            var subscriptionsCollection = SubscriptionsCollection.Parse(assembly);
            serviceCollection.AddSingleton(subscriptionsCollection);
            foreach (var controllerType in subscriptionsCollection.Select(c => c.Subscriber.Method.Type))
            {
                serviceCollection.AddScoped(controllerType);
            }
        }
    }
}