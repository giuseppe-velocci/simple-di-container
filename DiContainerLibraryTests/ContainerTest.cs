using DiContainerLibrary;
using System;
using System.Reflection;
using Xunit;

namespace DiContainerLibraryTests
{
    public class ContainerTest
    {
        private Container Sut { get; }
        public ContainerTest()
        {
            Sut = new Container();
        }

        [Fact]
        public void Should_Register_Instances_As_Singleton()
        {
            object instance = new object();
            Sut.RegisterSingleton<object>(instance);

            Assert.Equal(instance, Sut.Resolve<object>());
        }
        
        [Fact]
        public void Should_Register_Instances_Also_From_Interface_As_Singleton()
        {
            MyEmployeeWith2Ctor instance = new MyEmployeeWith2Ctor("Icabod");
            Sut.RegisterSingleton<IMyEmployee>(instance);

            Assert.Equal(instance, Sut.Resolve<IMyEmployee>());
        }
        
        [Fact]
        public void Should_Register_Instances_Combining_Ctors_From_Container_As_Singleton()
        {
            MyManager instance = new MyManager();
            Sut.RegisterSingleton<IMyManager>(instance);
            Sut.RegisterSingleton<IMyEmployee, MyManagedEmployee>();
            MyManagedEmployee resolved = Sut.Resolve<MyManagedEmployee>();
            IMyEmployee resolvedAsInterface = Sut.Resolve<IMyEmployee>();

            Assert.Equal(instance, Sut.Resolve<IMyManager>());
            Assert.Equal(instance.GetName(), resolved.GetManagerName());
            Assert.Equal(MyManagedEmployee.Name, resolvedAsInterface.GetName());
        }
        
        [Fact]
        public void Should_Fail_Registeration_If_Class_Has_More_Than_One_Ctor_As_Singleton()
        {
            Assert.Throws<AmbiguousMatchException>(() => Sut.RegisterSingleton<IMyEmployee, MyEmployeeWith2Ctor>());
            Assert.Throws<AmbiguousMatchException>(() => Sut.RegisterSingleton<MyEmployeeWith2Ctor>());
        }

        [Fact]
        public void Should_Fail_Registeration_As_Singleton_If_Class_Has_Transient_Dependencies()
        {
            Sut.RegisterTransient<IMyManager, MyManager>();
            Assert.Throws<ArgumentException>(() => Sut.RegisterSingleton<IMyEmployee, MyManagedEmployee>());
            Assert.Throws<ArgumentException>(() => Sut.RegisterSingleton<MyManagedEmployee>());
        }

        [Fact]
        public void Should_Register_As_Transient_And_Resolve_New_Instances()
        {
            Sut.RegisterTransient<IMyManager, MyManager>();
            Sut.RegisterTransient<MyEmployee>();

            var manager1 = Sut.Resolve<MyManager>();
            var manager2 = Sut.Resolve<MyManager>();

            var employee1 = Sut.Resolve<MyManager>();
            var employee2 = Sut.Resolve<MyManager>();

            Assert.NotEqual(manager1, manager2);
            Assert.NotEqual(employee1, employee2);
        }

        [Fact]
        public void Should_Return_Null_Resolving_Unregistered_Instances_As_Singleton()
        {
            Assert.Null(Sut.Resolve<object>());
        }
    }

    interface IMyEmployee
    {
        string GetName();
    }

    class MyEmployee : IMyEmployee
    {
        public string GetName()
        {
           return "Employee";
        }
    }

    class MyEmployeeWith2Ctor : IMyEmployee
    {
        string Name { get; }

        public MyEmployeeWith2Ctor(string name)
        {
            Name = name;
        }
        
        public MyEmployeeWith2Ctor()
        {
            Name = "rookie";
        }

        public string GetName()
        {
            return Name;
        }
    }

    interface IMyManager : IMyEmployee { }

    class MyManager : IMyManager
    {
       public string GetName() => "Boss";
    }

    class MyManagedEmployee : IMyEmployee
    {
        private IMyManager Manager { get; }
        public static string Name { get; } = "name";

        public MyManagedEmployee(IMyManager manager)
        {
            Manager = manager;
        }

        public string GetManagerName() => Manager.GetName();

        public string GetName()
        {
            return Name;
        }
    }
}