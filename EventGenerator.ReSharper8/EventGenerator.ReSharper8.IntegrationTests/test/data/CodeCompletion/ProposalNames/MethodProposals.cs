using System;

namespace CodeExamples.CompletionProposals
{
    /// <summary>
    /// Examples of methods with different (combinations of) parameter(s), return type, and modifiers.
    /// </summary>
    public class MethodProposals
    {
        public void TriggerCompletionHerein()
        {
            this.My{caret}
        }

        // not covered by this test, as completion is triggered on this.$
        // see StaticMemberProposals.cs
        public static void MyStaticMethod()
        {
            
        }

        public Object MyMethodWithReturnType()
        {
            return null;
        }

        public int MyMethodWithAliasedReturnType()
        {
            return 0;
        }

        public void MyMethodWithParameter(Object param)
        {
            
        }

        public void MyMethodWithRefParameter(ref int i)
        {
            
        }

        public void MyMethodWithOutParameter(out bool b)
        {
            b = false;
        }

        public void MyMethodWithParamArray(params Object[] objs)
        {
            
        }

        public void MyMethodWithOptionalParameter(Object obj = null)
        {
            
        }
    }
}
