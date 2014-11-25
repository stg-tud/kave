
                using System;
                using System.Collections.Generic;
                using System.IO;
                
                
                namespace N {
                    public class C {
                        
                public void A(object o)
                {
                    var i = 0;
                    string s;
                    while((s = o.ToString()) != null) {
                        i++;
                        {caret}
                    }
                }
            
                    }
                }