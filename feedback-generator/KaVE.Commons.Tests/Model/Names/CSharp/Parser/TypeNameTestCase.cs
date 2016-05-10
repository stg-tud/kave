using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaVE.Commons.Tests.Model.Names.CSharp.Parser
{
    class TypeNameTestCase
    {
        private string identifier;
	    private string n;
	    private string assembly;

	    public TypeNameTestCase(string identifier, string n, string assembly) {
		    this.identifier = identifier;
		    this.n = n;
		    this.assembly = assembly;
	    }

        public string GetIdentifier()
        {
            return identifier;
        }

        public string GetNamespace()
        {
            return n;
        }

        public string GetAssembly()
        {
            return assembly;
        }
    }
}
