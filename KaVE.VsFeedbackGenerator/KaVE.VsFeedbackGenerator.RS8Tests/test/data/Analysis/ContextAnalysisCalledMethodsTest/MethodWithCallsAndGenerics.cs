using System.Collections;

namespace N
{
    interface I<TI1>
    {
        public TI1 Get();
        public TI2 Get<TI2>();
    }

    class C
    {
        void M<TM1, TM2>(I<TM1> i1, TM2 p) where TM2 : I<object>
        {
            i1.Get();        // (1)
            i1.Get<int>();   // (2)

            I<string> i2;
            i2.Get();        // (3)

            p.GetHashCode(); // (4)

            {caret}
        }
    }
}