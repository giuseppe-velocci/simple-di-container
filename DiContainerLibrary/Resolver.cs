using System;

namespace DiContainerLibrary
{
    class Resolver
    {
        public Func<object> Resolve { get; }
        public ResolverType ResolverType { get; }

        public Resolver(Func<object> resolve, ResolverType type)
        {
            Resolve = resolve;
            ResolverType = type;
        }
    }

    enum ResolverType
    {
        Singleton,
        Transient
    }
}
