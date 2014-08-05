namespace N
{
    interface I
    {
        void M(int i);
    }

    class C1 : I
    {
        public void M(int i)
        {
            {caret}
        }
    }
}
