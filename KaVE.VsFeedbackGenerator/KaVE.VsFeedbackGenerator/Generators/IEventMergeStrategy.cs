using KaVE.Model.Events;

namespace KaVE.VsFeedbackGenerator.Generators
{
    interface IEventMergeStrategy
    {
        bool Mergable(IDEEvent event1, IDEEvent event2);
        IDEEvent Merge(IDEEvent event1, IDEEvent event2);
    }
}