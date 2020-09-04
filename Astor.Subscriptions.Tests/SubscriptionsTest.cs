using System;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Example.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Astor.Subscriptions.Tests
{
    [TestClass]
    public class SubscriptionsTest
    {
        [TestMethod]
        public async Task ShouldHandleEvent()
        {
            var rabbitConnectionFactory = new ConnectionFactory();
            var connection = rabbitConnectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare("needs-greeting", "fanout");
            

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(channel);
            serviceCollection.AddSingleton<SubscriptionsService>();
            serviceCollection.AddSubscriptions(typeof(Example.Application.Greeting).Assembly);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var subscriptionsService = serviceProvider.GetRequiredService<SubscriptionsService>();
            await subscriptionsService.StartAsync(new CancellationTokenSource().Token);
            
            var payload = new
            {
                name = "Alex"
            };

            var json = JsonConvert.SerializeObject(payload);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish("needs-greeting", "", null, jsonBytes);
            
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
            while (Greeting.Phrase == null)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
            
            Assert.AreEqual("Hello, Alex", Greeting.Phrase);
        }
    }
}