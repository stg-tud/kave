namespace N
{
    class C1 { }

    class C2 : C1
    {
        public virtual void M(int i) { }
    }

    class C3 : C2
    {
        public override void M(int i)
        {
            {caret}
        }
    }
}