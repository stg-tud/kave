namespace TestNamespace
{

    public interface AnInterface {}

    public class SuperClass {}

    public class TestClass : SuperClass, AnInterface
    {
        public void Doit()
        {
            this.Doit();
            {caret}
        }
    }
}