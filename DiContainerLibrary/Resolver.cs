using System;
using System.Collections.Generic;
using System.Linq;

namespace DiContainerLibrary
{
    class Resolver
    {
        public Func<object> Resolve { get; }
        public Lifecycle Lifecycle { get; }

        private Resolver(Func<object> resolve, Lifecycle type)
        {
            Resolve = resolve;
            Lifecycle = type;
        }

        public static Resolver SingletonResolver(object instance)
        {
            return new Resolver(() => instance, Lifecycle.Singleton);
        }

        public static Resolver TransientResolver(Container container, ConstructorData ctorData)
        {
            return new Resolver(() =>
            {
                object[] parameterInstances = ctorData.Parameters.Select(x => container.Resolve(x.ParameterType)).ToArray();
                return ctorData.Ctor.Invoke(parameterInstances);
            },
            Lifecycle.Transient);
        }
    }

    enum Lifecycle
    {
        Singleton,
        Transient
    }
}
