using Astor.Subscriptions.Abstractions;

namespace Astor.Subscriptions
{
    public class Subscription
    {
        public SubscribedOnAttribute SubscribedOnAttribute { get; }
        public Subscriber Subscriber { get; }

        public Subscription(SubscribedOnAttribute subscribedOnAttribute, Subscriber subscriber)
        {
            this.SubscribedOnAttribute = subscribedOnAttribute;
            this.Subscriber = subscriber;
        }
    }
}