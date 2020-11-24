using System;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Example.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Astor.Subscriptions.Tests.Old
{
    [TestClass]
    public class SubscriptionsTest
    {
        private static string[] names = new[]
        {
            "Alex",
            "Adam",
            "Anastasia",
            "Nikolas",
            "John"
        };
        
        [TestMethod]
        public async Task ShouldHandleEvent()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("Rabbit:Host", "localhost"),
                new KeyValuePair<string, string>("Rabbit:Port", "5672"),
                new KeyValuePair<string, string>("Rabbit:Password", "guest"),
                new KeyValuePair<string, string>("Rabbit:Login", "guest") 
            }).Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddRabbit(configuration.GetSection("Rabbit"));
            serviceCollection.AddSingleton<SubscriptionsService>();
            serviceCollection.AddSubscriptions(typeof(Greeting).Assembly);
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var channel = serviceProvider.GetRequiredService<IModel>();
            channel.ExchangeDeclare("needs-greeting", "fanout");
            
            var subscriptionsService = serviceProvider.GetRequiredService<SubscriptionsService>();
            await subscriptionsService.StartAsync(new CancellationTokenSource().Token);
            
            
            channel.PublishJson("needs-greeting", new
            {
                name = "Alex"
            });

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

            while (Greeting.Phrase == null)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            
            Assert.AreEqual("Hello, Alex", Greeting.Phrase);
        }
    }
}