namespace TestNamespace
{

    public interface AnInterface
    {
        void Doit();
    }

    public class SuperClass : AnInterface
    {
        public virtual void Doit() {}
    }

    public class TestClass : SuperClass
    {
        public override void Doit()
        {
            this.Doit();
            {caret}
        }
    }
}