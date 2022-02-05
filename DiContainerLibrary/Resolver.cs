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

        public static Resolver SingletonResolver(object instance)
        {
            return new Resolver(() => instance, Lifecycle.Singleton);
        }

        public static Resolver TransientResolver(Func<object> constructor)
        {
            return new Resolver(() => constructor(), Lifecycle.Transient);
        }
    }

    enum Lifecycle
    {
        Singleton,
        Transient
    }
}
