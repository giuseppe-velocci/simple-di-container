using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace DiContainerLibrary
{
    public class Container
    {
        private IDictionary<Type, Resolver> Registry { get; }

        public Container()
        {
            Registry = new Dictionary<Type, Resolver>();
        }

        public void RegisterSingleton<AbstractType, ConcreteType>() where AbstractType : class where ConcreteType : class
        {
            Resolver result = GetResolver<ConcreteType>();
            if (result is null)
            {
                RegisterSingleton<ConcreteType>();
            }

            RegisterSingleton<AbstractType>(Resolve<ConcreteType>());
        }

        public void RegisterSingleton<InstanceType>() where InstanceType : class
        {
            var ctorData = GetConstructorData<InstanceType>();
            if (ctorData.Parameters.Any(x => IsResolverTransient(x.ParameterType)))
            {
                throw new ArgumentException($"Singleton class {ctorData.InstanceType.FullName} cannot have transient dependencies");
            }

            object[] instances = ctorData.Parameters.Select(x => Resolve(x.ParameterType)).ToArray();
            object instance = ctorData.Ctor.Invoke(instances);
            RegisterSingleton<InstanceType>(instance);
        }

        public void RegisterSingleton<InstanceType>(object instance) where InstanceType : class
        {
            var resolver = Resolver.SingletonResolver(instance);
            Add(typeof(InstanceType), resolver);
        }

        public void RegisterTransient<AbstractType, ConcreteType>() where AbstractType : class where ConcreteType : class
        {
            Resolver result = GetResolver<ConcreteType>();
            if (result is null)
            {
                RegisterTransient<ConcreteType>();
            }

            Resolver transientResolver = GetResolver<ConcreteType>();
            Add(typeof(AbstractType), transientResolver);
        }

        public void RegisterTransient<InstanceType>()
        {
            var ctorData = GetConstructorData<InstanceType>();
            if (ctorData.Parameters.Any(x => ! Registry.ContainsKey(x.ParameterType)))
            {
                throw new NullReferenceException();
            }

            Resolver resolver = Resolver.TransientResolver(this, ctorData);
            Add(ctorData.InstanceType, resolver);
        }

        public InstanceType Resolve<InstanceType>() where InstanceType : class
        {
            return (InstanceType)Resolve(typeof(InstanceType));
        }

        public object Resolve(Type instanceType)
        {
            Resolver resolver;
            Registry.TryGetValue(instanceType, out resolver);
            if (resolver is null)
            {
                throw new NullReferenceException();
            }

            return resolver.Resolve();
        }

        private ConstructorData GetConstructorData<ConcreteType>()
        {
            var concreteType = typeof(ConcreteType);
            ConstructorInfo[] ctors = concreteType.GetConstructors();
            if (ctors.Count() > 1)
            {
                throw new AmbiguousMatchException($"Cannot register {concreteType.FullName}. It has more than one constructor.");
            }
            var ctor = ctors.First();

            return new ConstructorData(concreteType, ctor, ctor.GetParameters());
        }

        private Resolver GetResolver<InstanceType>()
        {
            Resolver result;
            var concreteType = typeof(InstanceType);
            Registry.TryGetValue(concreteType, out result);
            return result;
        }

        private bool IsResolverTransient(Type instanceType)
        {
            Resolver resolver;
            Registry.TryGetValue(instanceType, out resolver);
            return resolver.Lifecycle is Lifecycle.Transient;
        }

        private void Add(Type type, Resolver resolver)
        {
            Registry.Add(type, resolver);
        }
    }
}
