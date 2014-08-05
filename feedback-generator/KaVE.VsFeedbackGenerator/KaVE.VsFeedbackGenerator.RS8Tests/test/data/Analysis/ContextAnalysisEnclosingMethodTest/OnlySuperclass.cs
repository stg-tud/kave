namespace N
{
    class C1
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
