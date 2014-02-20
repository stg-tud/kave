using System;
using System.IO;

namespace CodeExamples.CompletionProposals
{
    /// <summary>
    /// Examples of proposals including array types.
    /// </summary>
    public class ArrayTypeProposals
    {
        private string[] myStringArray;
        private object[,,] myMultidimensionalArray;
        private object[][][] myJaggedArray;

        private void myMethod<R>(R[] p) {}

        public void TriggerCompletionHerein()
        {
            this.my{caret}
        }
    }
}
