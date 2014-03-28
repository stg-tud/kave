using System.Collections.Generic;
using KaVE.Model.Names;

namespace KaVE.Model.Groum
{
    public class CallGroum : GroumBase
    {
        public CallGroum(IMethodName calledMethod)
        {
            CalledMethod = calledMethod;
        }

        public IMethodName CalledMethod { get; private set; }

        public List<Instance> Parameter { get; set; }

        public Instance Return { get; set; }
    }
}