using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Astor.Subscriptions
{
    public static class DependencyInjectionHelper
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

        public static void AddRabbit(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<ConnectionFactory>(configuration);
            serviceCollection.AddSingleton(sp => sp.GetRequiredService<IOptions<ConnectionFactory>>().Value.CreateConnection());
            serviceCollection.AddSingleton(sp => sp.GetRequiredService<IConnection>().CreateModel());
        }
    }
}