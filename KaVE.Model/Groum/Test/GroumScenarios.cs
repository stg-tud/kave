using KaVE.Model.Names;

namespace KaVE.Model.Groum.Test
{
    class SomeClass
    {
        public bool IsTrue()
        {
            return true;
        }

        public void Execute()
        {
            
        }

        public SomeClass GetMe()
        {
            return this;
        }
    }

    internal class GroumTests
    {
        private SomeClass _mySomeClass;

        public void Case0()
        {
            var testVictim = new SomeClass();
            if (testVictim.IsTrue())
            {
                testVictim.Execute();
            }
        }

        public void Case0Solution0()
        {

            /*var scNew = new CallGroum(new Mock<IMethodName>().Object);
            var instance = new Instance(new Mock<ITypeName>().Object);
            var isTrue = new CallGroum(new Mock<IMethodName>().Object);
            var exec = new CallGroum(new Mock<IMethodName>().Object);
            var ifGroum = IfElseGroum.Create();

            scNew.Return = instance;
            scNew.Next = isTrue;
            isTrue.Next = ifGroum;
            isTrue.Parameter.Insert(0, instance);
            ifGroum.Then = exec;
            exec.Parameter.Insert(0, instance);
            */
            // var entry = GroumEntry.Create();
            // entry.SetRoot(scNew);
            // entry.AddField(FieldInstance);
            // entry.Parameter.Insert(0, ParamInstance);
        }

        public void Case1()
        {
            var testVictim = new SomeClass();
            var condition = testVictim.IsTrue();
            if (condition)
            {
                testVictim.Execute();
            }
        }

        public void Case2(bool condition)
        {
            var testVictim = new SomeClass();
            if (condition)
            {
                testVictim.Execute();
            }
        }

        public void Case3()
        {
            var someClass = new SomeClass();
            if (someClass.IsTrue())
            {
                someClass = new SomeClass();
            }
            someClass.Execute();
        }

        public void Case4(SomeClass me2)
        {
            if (_mySomeClass.IsTrue())
            {
                _mySomeClass.Execute();
            }
            me2.Execute(); 
        }

        private SomeClass _o;

        public void Case5()
        {
            var s = new SomeClass();
            _o = s.GetMe();
        }

        // Variables without outgoing edges are ignored.
        public void Case6()
        {
            var s = new SomeClass();
            SomeClass o;
        }
    }
}
