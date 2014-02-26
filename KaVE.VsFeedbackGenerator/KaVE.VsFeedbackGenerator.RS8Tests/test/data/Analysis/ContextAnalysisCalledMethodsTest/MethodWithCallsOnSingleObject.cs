using System.Collections;

namespace N
{
    class C1
    {
        public int M() { }
    }

    interface I1
    {
        bool Is();
        C1 GetC();
        void Do(int i);
    }

    class C2
    {
        void M(I1 i)
        {
            C1 c;
            if (i.Is())
            {
                c = i.GetC();
            }
            else
            {
                c = new C1();
            }
            i.Do(c.M());
            {caret}
        }
    }
}