namespace N
{
    interface I
    {
        void M();
    }

    class S
    {
        public virtual void M() {}
    }

    class C : S
    {
        public override void M() {}
        public void N() {}
    }
}