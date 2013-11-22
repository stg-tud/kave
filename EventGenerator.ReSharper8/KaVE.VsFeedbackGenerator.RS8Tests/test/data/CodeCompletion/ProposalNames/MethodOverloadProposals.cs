using System;

namespace CodeExamples.CompletionProposals
{
    /// <summary>
    /// Example of completion of overloaded method.
    /// </summary>
    public class MethodOverloadProposals
    {
        public void TriggerCompletionHerein()
        {
            // the first of the overloads is proposed in this case, i.e.,
            // the one with the int parameter.
            this.My{caret}
        }

        private void MyMethod(int i) { }
        private void MyMethod() {}
        private void MyMethod(string s) {}
    }
}
