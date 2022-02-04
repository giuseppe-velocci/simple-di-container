using System;

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

        public static Resolver SingletonResolver(Container container, ConstructorData ctorData)
        {
            object instance = ctorData.BuildInstance(container);
            return SingletonResolver(instance);
        }

        public static Resolver SingletonResolver(object instance)
        {
            return new Resolver(() => instance, Lifecycle.Singleton);
        }

        public static Resolver TransientResolver(Container container, ConstructorData ctorData)
        {
            return new Resolver(() => ctorData.BuildInstance(container), Lifecycle.Transient);
        }
    }

    enum Lifecycle
    {
        Singleton,
        Transient
    }
}
