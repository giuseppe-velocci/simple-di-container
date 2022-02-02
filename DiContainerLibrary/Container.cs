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

        public void RegisterSingleton<ConcreteType>(object instance) where ConcreteType : class
        {
            var resolver = new Resolver(() => instance, ResolverType.Singleton);
            Registry.Add(new KeyValuePair<Type, Resolver>(typeof(ConcreteType), resolver));
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

        public void RegisterSingleton<ConcreteType>() where ConcreteType : class
        {
            var ctorData = GetConstructorData<ConcreteType>();
            if (ctorData.Parameters.Any(x => IsResolverSingleton(x.ParameterType)))
            {
                throw new ArgumentException($"Singleton class {ctorData.ConcreteType.FullName} cannot have transient dependencies");
            }

            object[] instances = ctorData.Parameters.Select(x => Resolve(x.ParameterType)).ToArray();
            object instance = ctorData.Ctor.Invoke(instances);
            RegisterSingleton<ConcreteType>(instance);
        }

        public void RegisterTransient<AbstractType, ConcreteType>() where AbstractType : class where ConcreteType : class
        {
            Resolver result = GetResolver<ConcreteType>();
            if (result is null)
            {
                RegisterTransient<ConcreteType>();
            }
            Registry.Add(typeof(AbstractType), new Resolver(() => Resolve<ConcreteType>(), ResolverType.Transient));
        }

        public void RegisterTransient<ConcreteType>()
        {
            var ctorData = GetConstructorData<ConcreteType>();
            Resolver resolver = new Resolver(() =>
            {
                object[] instances = ctorData.Parameters.Select(x => Resolve(x.ParameterType)).ToArray();
                return ctorData.Ctor.Invoke(instances);
            }, 
            ResolverType.Transient);
            Registry.Add(ctorData.ConcreteType, resolver);
        }

        public ConcreteType Resolve<ConcreteType>() where ConcreteType : class
        {
            return (ConcreteType)Resolve(typeof(ConcreteType));
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

        private Resolver GetResolver<ConcreteType>()
        {
            Resolver result;
            var concreteType = typeof(ConcreteType);
            Registry.TryGetValue(concreteType, out result);
            return result;
        }

        private bool IsResolverSingleton(Type concreteType)
        {
            Resolver resolver;
            Registry.TryGetValue(concreteType, out resolver);
            return resolver.ResolverType is ResolverType.Transient;
        }

        private object Resolve(Type instanceType)
        {
            Resolver result;
            Registry.TryGetValue(instanceType, out result);
            return result?.Resolve();
        }
    }
}
