using System;
using System.Linq;
using System.Collections.Generic;

namespace DiContainerLibrary
{
    public class Container
    {
        private IRegistry Registry { get; }

        public Container()
        {
            Registry = new ContainerRegistry();
        }

        public void RegisterSingleton<AbstractType, ConcreteType>() where AbstractType : class where ConcreteType : class
        {
            Register<AbstractType, ConcreteType>(() => RegisterSingleton<ConcreteType>());
        }

        public void RegisterSingleton<InstanceType>() where InstanceType : class
        {
            var ctorData = ConstructorData.InitializeConstructorData<InstanceType>();
            if (ctorData.Parameters.Any(x => Registry.IsResolverTransient(x)))
            {
                throw new ArgumentException($"Singleton class {ctorData.InstanceType.FullName} cannot have transient dependencies");
            }

            object instance = ctorData.BuildInstance(this);
            RegisterSingleton<InstanceType>(instance);
        }

        public void RegisterSingleton<InstanceType>(object instance) where InstanceType : class
        {
            var resolver = Resolver.SingletonResolver(instance);
            Registry.Add(typeof(InstanceType), resolver);
        }

        public void RegisterTransient<AbstractType, ConcreteType>() where AbstractType : class where ConcreteType : class
        {
            Register<AbstractType, ConcreteType>(() => RegisterTransient<ConcreteType>());
        }

        public void RegisterTransient<InstanceType>()
        {
            var ctorData = ConstructorData.InitializeConstructorData<InstanceType>();
            if (ctorData.Parameters.Any(x => ! Registry.TypeExists(x)))
            {
                throw new NullReferenceException();
            }

            Resolver resolver = Resolver.TransientResolver(this, ctorData);
            Registry.Add(ctorData.InstanceType, resolver);
        }

        public InstanceType Resolve<InstanceType>() where InstanceType : class
        {
            return (InstanceType)Resolve(typeof(InstanceType));
        }

        internal object Resolve(Type instanceType)
        {
            Resolver resolver = Registry.FindResolver(instanceType);
            return resolver.Resolve();
        }

        private void Register<AbstractType, ConcreteType>(Action registerConctreteType) where AbstractType : class where ConcreteType : class
        {
            Resolver resolver = Registry.FindResolver<ConcreteType>();
            if (resolver is null)
            {
                registerConctreteType();
            }

            Resolver abstractTypeResolver = resolver is null ? Registry.FindResolver<ConcreteType>() : resolver;
            Registry.Add(typeof(AbstractType), abstractTypeResolver);
        }
    }
}
