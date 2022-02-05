using System;

namespace DiContainerLibrary
{
    interface IRegistry
    {
        Resolver FindResolver(Type instanceType);
        bool IsResolverTransient(Type instanceType);
        void Add(Type instanceType, Resolver resolver);
        bool TypeExists(Type instanceType);
    }
}
