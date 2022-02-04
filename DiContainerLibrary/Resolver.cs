using System;
using System.Linq;

namespace DiContainerLibrary
{
    class Resolver
    {
        public Func<object> Resolve { get; }
        public Lifecycle Lifecycle { get; }

        public Resolver(Func<object> resolve, Lifecycle type)
        {
            Resolve = resolve;
            Lifecycle = type;
        }
    }

    enum Lifecycle
    {
        Singleton,
        Transient
    }
}
