﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KaVE.VsFeedbackGenerator.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class PopupNotification {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PopupNotification() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KaVE.VsFeedbackGenerator.Properties.PopupNotification", typeof(PopupNotification).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bitte laden Sie ihr Feedback hoch. Das Feedback beansprucht aktuell {0:0.#} MB auf Ihrer Festplatte..
        /// </summary>
        public static string InformationHardpopup {
            get {
                return ResourceManager.GetString("InformationHardpopup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bitte laden Sie ihr Feedback hoch:.
        /// </summary>
        public static string InformationSoftpopup {
            get {
                return ResourceManager.GetString("InformationSoftpopup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Export-Erinnerung.
        /// </summary>
        public static string NotificationTitel {
            get {
                return ResourceManager.GetString("NotificationTitel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wizard starten....
        /// </summary>
        public static string WizardButton {
            get {
                return ResourceManager.GetString("WizardButton", resourceCulture);
            }
        }
    }
}
