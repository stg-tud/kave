namespace N
{
    interface I
    {
        void M(int i);
    }

    abstract class C1 : I
    {
        public virtual void M(int i) { }
    }

    class C2 : C1
    {
        public override void M(int i)
        {
            {caret}
        }
    }
}