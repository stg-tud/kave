using System;
using System.IO;

namespace CodeExamples.CompletionProposals
{
    /// <summary>
    /// Examples of different ways variables can be introduced.
    /// </summary>
    public class VariableProposals
    {
        private Object var_Field;

        public void Method(int var_Param)
        {
            var var_Str = "";
            for (var var_Index = 0; i < 10; i++)
            {
                try
                {
                    throw new Exception();
                }
                catch (Exception var_Exception)
                {
                    using (var var_Using = new MemoryStream())
                    {
                        var_{caret}
                    }
                }
            }
        }
    }
}
