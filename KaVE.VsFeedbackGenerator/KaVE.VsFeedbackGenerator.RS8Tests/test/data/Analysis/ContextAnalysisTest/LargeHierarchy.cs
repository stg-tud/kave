namespace N
{
    interface I0
    {
         
    }

    interface IA : I0
    {
         
    }

    interface IB<TB>
    {
        
    }

    interface IC
    {
        
    }

    class A : IA
    {
        
    }

    class B : A, IB<int>, IC
    {
        public void m()
        {
            {caret}
        }
    }
}