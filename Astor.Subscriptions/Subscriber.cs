using System;
using System.Linq;
using Empowered.Reflection;

namespace Astor.Subscriptions
{
    public class Subscriber
    {
        public string Id { get; }
        public Method Method { get; }
        public Type BodyType { get; }

        private Subscriber(string id, Method method, Type bodyType)
        {
            this.Id = id;
            this.Method = method;
            this.BodyType = bodyType;
        }

        public static Subscriber Create(Method method)
        {
            var methodNameForId = method.Info.Name.Replace("Async", "").ToLower();
            var controllerName = method.Type.Name.Replace("Controller", "").ToLower();

            var firstParameterType = method.Info.GetParameters().FirstOrDefault()?.ParameterType;
            
            return new Subscriber($"{controllerName}_{methodNameForId}", method, firstParameterType);
        }
    }
}