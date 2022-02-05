using System;
using System.Linq;

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
            var ctorData = GetSingletonConstructorData<InstanceType>();
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
            var ctorData = GetConstructorData<InstanceType>();
            Resolver resolver = Resolver.TransientResolver(() => ctorData.BuildInstance(this));
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

        private ConstructorData GetSingletonConstructorData<InstanceType>()
        {
            var ctorData = GetConstructorData<InstanceType>();
            if (ctorData.ConstructorParameters.Any(x => Registry.IsResolverTransient(x)))
            {
                throw new ArgumentException($"Singleton class {ctorData.InstanceType.FullName} cannot have transient dependencies");
            }
            return ctorData;
        }

        private ConstructorData GetConstructorData<InstanceType>()
        {
            var ctorData = ConstructorData.InitializeConstructorData<InstanceType>();
            if (ctorData.ConstructorParameters.Any(x => !Registry.TypeExists(x)))
            {
                throw new NullReferenceException(
                    $"Class {ctorData.InstanceType.FullName} is missing one or more dependencies: " +
                    $"{ctorData.ConstructorParameters.Where(x => !Registry.TypeExists(x))}"
                    );
            }
            return ctorData;
        }

        private void Register<AbstractType, ConcreteType>(Action registerConctreteType) where AbstractType : class where ConcreteType : class
        {
            var concreteType = typeof(ConcreteType);
            Resolver resolver = Registry.FindResolver(concreteType);
            if (resolver is null)
            {
                registerConctreteType();
            }

            Resolver abstractTypeResolver = resolver is null ? Registry.FindResolver(concreteType) : resolver;
            Registry.Add(typeof(AbstractType), abstractTypeResolver);
        }
    }
}
