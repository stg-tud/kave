namespace N
{
    interface I1
    {
        void M(int i);
    }

    interface I2
    {
        void M(int i);
    }

    class C1 : I1
    {
        public virtual void M(int i) { }
    }

    class C2 : C1, I2
    {
        public override void M(int i)
        {
            {caret}
        }
    }
}