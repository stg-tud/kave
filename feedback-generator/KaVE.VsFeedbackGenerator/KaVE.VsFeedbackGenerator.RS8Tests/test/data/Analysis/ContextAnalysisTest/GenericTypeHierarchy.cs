namespace N
{
    class IC<T>
    {
        
    }

    class C<T> : IC<int>
    {
        void M()
        {
            {caret}
        }
    }
}