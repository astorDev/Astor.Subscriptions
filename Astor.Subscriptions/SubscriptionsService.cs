using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Empowered.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Astor.Subscriptions
{
    public class SubscriptionsService : IHostedService
    {
        public IModel RabbitChannel { get; }
        public IServiceProvider ServiceProvider { get; }
        public SubscriptionsCollection Subscriptions { get; }

        public SubscriptionsService(IModel rabbitChannel, 
            IServiceProvider serviceProvider, 
            SubscriptionsCollection subscriptionsCollection)
        {
            this.RabbitChannel = rabbitChannel;
            this.ServiceProvider = serviceProvider;
            this.Subscriptions = subscriptionsCollection;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var subscription in this.Subscriptions)
            {
                this.RabbitChannel.QueueDeclare(subscription.Subscriber.Id, true, false, false);
                this.RabbitChannel.QueueBind(subscription.Subscriber.Id, subscription.SubscribedOnAttribute.Event, "");
                
                var consumer = new EventingBasicConsumer(this.RabbitChannel);
                consumer.Received += async (sender, args) =>
                {
                    try
                    {
                        var json = Encoding.UTF8.GetString(args.Body.ToArray());
                        
                        var candidate = JsonConvert.DeserializeObject(json, subscription.Subscriber.BodyType);

                        using var scope = this.ServiceProvider.CreateScope();
                        var controller = scope.ServiceProvider.GetRequiredService(subscription.Subscriber.Method.Type);

                        await subscription.Subscriber.Method.Info.InvokeAsync(controller, candidate);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                };

                this.RabbitChannel.BasicConsume(subscription.Subscriber.Id, true, consumer);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}