
                using System;
                using System.Collections.Generic;
                using System.IO;
                
                
                namespace N
                {
                    class C1
                    {
                        virtual void M() {}
                    }

                    class C2 : C1
                    {
                        override void M() {}
                    }

                    class C2 : C1
                    {
                        {caret}
                    }
                }               
            