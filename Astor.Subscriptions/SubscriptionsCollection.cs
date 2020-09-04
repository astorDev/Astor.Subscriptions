using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Astor.Subscriptions.Abstractions;
using Empowered.Reflection;

namespace Astor.Subscriptions
{
    public class SubscriptionsCollection : IEnumerable<Subscription>
    {
        private readonly IEnumerable<Subscription> subscriptions;

        public SubscriptionsCollection(IEnumerable<Subscription> subscriptions)
        {
            this.subscriptions = subscriptions;
        }
        
        public static SubscriptionsCollection Parse(Assembly assembly)
        {
            return new SubscriptionsCollection(parse(assembly));
        }

        private static IEnumerable<Subscription> parse(Assembly assembly)
        {
            foreach (var controllerType in getControllerTypes(assembly))
            {
                var subscriptionAttributes = from m in controllerType.GetMethods() 
                    from a in m.GetCustomAttributes(typeof(SubscribedOnAttribute)) 
                    select new
                    {
                        Method = m,
                        Attribute = (SubscribedOnAttribute) a
                    };
                
                foreach (var attribute in subscriptionAttributes)
                {
                    yield return new Subscription(attribute.Attribute, Subscriber.Create(new Method(controllerType, attribute.Method)));
                }
                    
            }
        }
        
        private static IEnumerable<Type> getControllerTypes(Assembly assembly)
        {
            foreach (var controllerType in assembly.DefinedTypes.Where(t => t.Name.EndsWith("Controller")))
            {
                yield return controllerType;
            }
            
            var referencedAssemblies = assembly.GetReferencedAssemblies()
                .Select(a => Assembly.Load(a.Name));

            foreach (var referencedAssembly in referencedAssemblies)
            {
                foreach (var controllerType in referencedAssembly.DefinedTypes.Where(t => t.Name.EndsWith("Controller")))
                {
                    yield return controllerType;
                }
            }
        }

        public IEnumerator<Subscription> GetEnumerator()
        {
            return this.subscriptions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}