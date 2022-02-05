using System;
using System.Collections.Generic;

namespace DiContainerLibrary
{
    class ContainerRegistry : IRegistry
    {
        private IDictionary<Type, Resolver> Registry { get; }

        public ContainerRegistry()
        {
            Registry = new Dictionary<Type, Resolver>();
        }

        public void Add(Type instanceType, Resolver resolver)
        {
            Registry.Add(instanceType, resolver);
        }

        public Resolver FindResolver(Type instanceType)
        {
            Resolver resolver;
            Registry.TryGetValue(instanceType, out resolver);
            return resolver;
        }

        public bool IsResolverTransient(Type instanceType)
        {
            Resolver resolver;
            Registry.TryGetValue(instanceType, out resolver);
            return resolver.Lifecycle is Lifecycle.Transient;
        }

        public bool TypeExists(Type instanceType)
        {
            return Registry.ContainsKey(instanceType);
        }
    }
}
