namespace DiContainer
{
    class ClassC
    {
        public IClassB B { get; set; }

        public ClassC(IClassB b)
        {
            B = b;
        }
    }
}
