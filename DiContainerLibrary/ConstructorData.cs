using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DiContainerLibrary
{
    class ConstructorData
    {
        public Type InstanceType { get; }
        public ConstructorInfo Ctor { get; }
        public Type[] Parameters { get; }

        private ConstructorData(
            Type instanceType,
            ConstructorInfo ctor,
            Type[] parameters
        )
        {
            InstanceType = instanceType;
            Ctor = ctor;
            Parameters = parameters;
        }

        public static ConstructorData InitializeConstructorData<ConcreteType>()
        {
            var concreteType = typeof(ConcreteType);
            ConstructorInfo[] ctors = concreteType.GetConstructors();
            if (ctors.Count() > 1)
            {
                throw new AmbiguousMatchException($"Invalid class {concreteType.FullName}: it has more than one constructor.");
            }
            var ctor = ctors.First();
            Type[] parameters = ctor.GetParameters().Select(x => x.ParameterType).ToArray();
            return new ConstructorData(concreteType, ctor, parameters);
        }

        public object BuildInstance(Container container)
        {
            var parameterInstances = Parameters.Select(x => container.Resolve(x)).ToArray();
            return Ctor.Invoke(parameterInstances);
        }
    }
}
