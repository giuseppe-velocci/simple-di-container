using System;

namespace DiContainer
{
    class ClassB : IClassB
    {
        public ClassA A { get; set; }

        public ClassB(ClassA a)
        {
            A = a;
        }

        public void Hello()
        {
            Console.WriteLine("Hello!");
        }
    }
}
