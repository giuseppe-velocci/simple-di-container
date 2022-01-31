using DiContainerLibrary;
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
        public void Should_Register_Instances()
        {
            object instance = new object();
            Sut.Register<object>(instance);

            Assert.Equal(instance, Sut.Resolve<object>());
        }
        
        [Fact]
        public void Should_Register_Instances_Also_From_Interface()
        {
            MyEmployee instance = new MyEmployee("Icabod");
            Sut.Register<IMyEmployee>(instance);

            Assert.Equal(instance, Sut.Resolve<IMyEmployee>());
        }
        
        [Fact]
        public void Should_Register_Instances_Combinging_Ctors_From_Container()
        {
            MyManager instance = new MyManager();
            Sut.Register<IMyManager>(instance);
            Sut.Register<IMyEmployee, MyManagedEmployee>();
            MyManagedEmployee resolved = (MyManagedEmployee)Sut.Resolve<IMyEmployee>();

            Assert.Equal(instance, Sut.Resolve<IMyManager>());
            Assert.Equal(instance.GetName(), resolved.GetManagerName());
        }
        
        [Fact]
        public void Should_Fail_Registeration_If_Class_Has_More_Than_One_Ctor()
        {
            Assert.Throws<AmbiguousMatchException>(() => Sut.Register<IMyEmployee, MyEmployee>());
        }

        [Fact]
        public void Should_Return_Null_Resolving_Unregistered_Instances()
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
        string Name { get; }

        public MyEmployee(string name)
        {
            Name = name;
        }
        
        public MyEmployee()
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

    class MyManagedEmployee
    {
        private IMyManager Manager { get; }
        public MyManagedEmployee(IMyManager manager)
        {
            Manager = manager;
        }

        public string GetManagerName() => Manager.GetName();
    }

}