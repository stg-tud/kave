
                using System;
                using System.Collections.Generic;
                using System.IO;
                
                
                class C
                {
                    private void M()
                    {
                        EP();
                    }
                    public void EP()
                    {
                        M();
                        {caret}
                    }
                }