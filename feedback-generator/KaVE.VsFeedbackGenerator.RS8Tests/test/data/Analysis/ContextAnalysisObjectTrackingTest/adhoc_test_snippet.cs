
                using System;
                using System.Collections.Generic;
                using System.IO;
                
                
                class C {
                    public void M1(object o) {
                        this.M2(o);
                        {caret}
                    }
        
                    private void M2(object o) {}
                }