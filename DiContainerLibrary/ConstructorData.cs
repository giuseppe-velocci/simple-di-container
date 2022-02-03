using System;
using System.Reflection;

namespace DiContainerLibrary
{
    class ConstructorData
    {
        public Type InstanceType { get; }
        public ConstructorInfo Ctor { get; }
        public ParameterInfo[] Parameters { get; }

        public ConstructorData(
            Type instanceType,
            ConstructorInfo ctor,
            ParameterInfo[] parameters
        )
        {
            InstanceType = instanceType;
            Ctor = ctor;
            Parameters = parameters;
        }
    }
}
