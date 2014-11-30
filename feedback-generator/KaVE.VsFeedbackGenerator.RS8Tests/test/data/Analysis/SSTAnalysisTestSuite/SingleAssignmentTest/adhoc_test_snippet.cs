
                using System;
                using System.Collections.Generic;
                using System.IO;
                
                
                namespace N {
                    public class H {
                        public int Get() {
                            return 1;
                        }
                    }
                    public class C {
                        public void A(H h) {
                            var i = 1 + h.Get();
                            {caret}
                        }
                    }
                }
            