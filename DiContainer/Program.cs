using DiContainerLibrary;
using System;

namespace DiContainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container();
            container.RegisterSingleton<ClassA>();
            container.RegisterSingleton<IClassB, ClassB>();
            container.RegisterTransient<ClassC>();

            var b = container.Resolve<ClassB>();
            var b1 = container.Resolve<ClassB>();

            var c = container.Resolve<ClassC>();
            var c1 = container.Resolve<ClassC>();

            Console.WriteLine($"Are singleton class {b.ToString()} instance equals? {b == b1}");
            Console.WriteLine($"Are transient class {c.ToString()} instance equals? {c == c1}");
            c.B.Hello();
            Console.ReadLine();
        }
    }
}
