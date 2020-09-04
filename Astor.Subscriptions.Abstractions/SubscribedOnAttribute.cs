using System;

namespace Astor.Subscriptions.Abstractions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubscribedOnAttribute : Attribute
    {
        public string Event { get; }

        public SubscribedOnAttribute(string @event)
        {
            this.Event = @event;
        }
    }
}