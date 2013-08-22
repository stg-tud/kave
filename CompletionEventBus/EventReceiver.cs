using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompletionEventBus
{
    public interface IEventReceiver<TEventType>
    {
        void Send(TEventType evt);
    }
}
