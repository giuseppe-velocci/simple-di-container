using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace DiContainerLibrary
{
    public class Container
    {
        private IDictionary<Type, object> Registry { get; }

        public Container()
        {
            Registry = new Dictionary<Type, object>();
        }

        public void Register<ConcreteType>(object instance) where ConcreteType : class
        {
            Registry.Add(new KeyValuePair<Type, object>(typeof(ConcreteType), instance));
        }

        public void Register<AbstractType, ConcreteType>() where AbstractType : class where ConcreteType : class
        {
            object result;
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
            int minCtorParams = ctors.Min(x => x.GetParameters().Length);
            var ctor = ctors
                .Where(x => x.GetParameters().Length == minCtorParams)
                .FirstOrDefault();
            ParameterInfo[] parameters = ctor.GetParameters();
            object[] instances = parameters.Select(x => Resolve(x.ParameterType)).ToArray();
            object instance = ctor.Invoke(instances);
            Register<ConcreteType>(instance);
        }

        public object Resolve<GenericType>() where GenericType : class
        {
            return Resolve(typeof(GenericType));
        }

        private object Resolve(Type instanceType)
        {
            object result;
            Registry.TryGetValue(instanceType, out result);
            return result;
        }
    }
}
