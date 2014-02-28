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

    class C : I2, I1
    {
        public void M(int i)
        {
            {caret}
        }
    }
}
