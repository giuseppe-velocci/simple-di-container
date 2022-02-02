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

        public void Register<ConcreteType>(object instance) where ConcreteType : class
        {
            var resolver = new Resolver(() => instance, ResolverType.Singleton);
            Registry.Add(new KeyValuePair<Type, Resolver>(typeof(ConcreteType), resolver));
        }

        public void Register<AbstractType, ConcreteType>() where AbstractType : class where ConcreteType : class
        {
            Resolver result;
            var concreteType = typeof(ConcreteType);
            Registry.TryGetValue(concreteType, out result);
            if (result is null)
            {
                RegisterWithDefaultCtor<ConcreteType>(concreteType);
            }
            Register<AbstractType>(Resolve<ConcreteType>());
        }

        private void RegisterWithDefaultCtor<ConcreteType>(Type concreteType) where ConcreteType : class
        {
            var ctors = concreteType.GetConstructors();
            if (ctors.Count() > 1)
            {
                throw new AmbiguousMatchException($"Cannot register {concreteType.FullName}. It has more than one constructor.");
            }

            var ctor = ctors.First();
            ParameterInfo[] parameters = ctor.GetParameters();
            object[] instances = parameters.Select(x => Resolve(x.ParameterType)).ToArray();
            object instance = ctor.Invoke(instances);
            Register<ConcreteType>(instance);
        }

        public GenericType Resolve<GenericType>() where GenericType : class
        {
            return (GenericType)Resolve(typeof(GenericType));
        }

        private object Resolve(Type instanceType)
        {
            Resolver result;
            Registry.TryGetValue(instanceType, out result);
            return result is null ? null : result.Resolve();
        }
    }
}
