using System;
using System.Reflection;

namespace DiContainerLibrary
{
    class ConstructorData
    {
        public Type ConcreteType { get; }
        public ConstructorInfo Ctor { get; }
        public ParameterInfo[] Parameters { get; }

        public ConstructorData(
            Type concreteType,
            ConstructorInfo ctor,
            ParameterInfo[] parameters
        )
        {
            ConcreteType = concreteType;
            Ctor = ctor;
            Parameters = parameters;
        }
    }
}
