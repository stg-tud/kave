namespace N
{
    public abstract class A
    {
        public abstract void M(int i);
    }

    public class C1 : A
    {
        public override void M(int i) { }
    }

    public class C2 : C1
    {
        public override void M(int i)
        {
            {caret}
        }
    }
}