using System.Collections;

namespace N
{
    interface I<TI1>
    {
        TI1 Get();
        TI2 Get<TI2>();
    }

    class C
    {
        void M<TM1, TM2>(I<TM1> i1, TM2 p, D<TM2> d) where TM2 : I<object>
        {
            i1.Get();        // (1)
            i1.Get<int>();   // (2)

            I<string> i2 = null;
            i2.Get();        // (3)

            p.GetHashCode(); // (4)

            d.Get();         // (5)

            {caret}
        }
    }

    class D<B> : I<B>
    {
        public B Get() { return default(B); }
        public TI2 Get<TI2>() { return default(TI2); }
    }
}