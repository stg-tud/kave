namespace N
{
    interface I
    {
        virtual void M(int i);
    }

    class C1 : I { }

    class C2 : C1
    {
        override void M(int i) { }
    }

    class C3 : C2 { }

    class C4 : C3
    {
        override void M(int i) { }
    }

    class C5 : C4
    {
        void M(double i) { }
    }

    class C6 : C5
    {
        override void M(int i)
        {
            {caret}
        }
    }
}
