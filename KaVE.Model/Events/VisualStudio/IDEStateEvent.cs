using System.Collections.Generic;
using System.Runtime.Serialization;
using KaVE.Model.Names.VisualStudio;

namespace KaVE.Model.Events.VisualStudio
{
    [DataContract]
    public class IDEStateEvent : IDEEvent
    {
        public enum LifecyclePhase
        {
            /// <summary>
            /// State snapshot is taken immediately after startup, i.e., before first user interactions.
            /// </summary>
            Startup,

            /// <summary>
            /// State snapshot is taken before shutdown, i.e., after the last user interaction.
            /// </summary>
            Shutdown,

            /// <summary>
            /// State snapshot is taken somewhen during runtime.
            /// </summary>
            Runtime
        }

        [DataMember]
        public LifecyclePhase IDELifecyclePhase { get; set; }

        [DataMember]
        public IList<WindowName> OpenWindows { get; set; }

        [DataMember]
        public IList<DocumentName> OpenDocuments { get; set; }
    }
}