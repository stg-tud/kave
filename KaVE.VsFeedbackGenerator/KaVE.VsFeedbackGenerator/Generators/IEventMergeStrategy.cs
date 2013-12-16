using KaVE.Model.Events;

namespace KaVE.VsFeedbackGenerator.Generators
{
    interface IEventMergeStrategy
    {
        bool Mergable(IDEEvent @event, IDEEvent subsequentEvent);
        IDEEvent Merge(IDEEvent @event, IDEEvent subsequentEvent);
    }
}