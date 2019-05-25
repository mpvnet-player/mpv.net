namespace mpvnet.Properties {
    using System;

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("mpvnet.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }

        internal static string inputConf {
            get {
                return ResourceManager.GetString("inputConf", resourceCulture);
            }
        }

        internal static string inputConfHeader {
            get {
                return ResourceManager.GetString("inputConfHeader", resourceCulture);
            }
        }
        
        internal static string mpvConf {
            get {
                return ResourceManager.GetString("mpvConf", resourceCulture);
            }
        }

        internal static string mpvConfToml {
            get {
                return ResourceManager.GetString("mpvConfToml", resourceCulture);
            }
        }

        internal static System.Drawing.Bitmap mpvnet {
            get {
                object obj = ResourceManager.GetObject("mpvnet", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        internal static string mpvNetConfToml {
            get {
                return ResourceManager.GetString("mpvNetConfToml", resourceCulture);
            }
        }
    }
}