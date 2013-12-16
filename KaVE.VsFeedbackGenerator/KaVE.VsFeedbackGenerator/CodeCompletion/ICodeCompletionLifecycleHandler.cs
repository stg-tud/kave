using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Lookup;
using KaVE.Model.Events;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    internal interface ICodeCompletionLifecycleHandler
    {
        /// <summary>
        /// Invoked when the code completion is opened.
        /// </summary>
        /// <param name="prefix">The prefix, the completion is triggered on.</param>
        void OnOpened(string prefix);

        void SetLookupItems(IEnumerable<ILookupItem> items);

        /// <summary>
        /// Invoked when the prefix changes (typing or deletion of a character), while the code completion is opened.
        /// </summary>
        /// <param name="newPrefix">The prefix after it was changed.</param>
        /// <param name="displayedLookupItems">The lookup items displayed after the change.</param>
        void OnPrefixChanged(string newPrefix, IEnumerable<ILookupItem> displayedLookupItems);

        /// <summary>
        /// Invoked for the initial selection, any manual selection changes (using the arrow keys), selection changes
        /// caused by filtering, and when an unselected item is clicked (which immediately applies the selected
        /// completion).
        /// </summary>
        void OnSelectionChanged(ILookupItem selectedItem);

        /// <summary>
        /// Called when the completion is closed. This happens for every completion, regardless of whether is is
        /// applied or cancelled. This is invoked before <see cref="OnApplication"/>.
        /// </summary>
        void OnClosed();

        /// <summary>
        /// Invoked when the code completion is closed due to the application of an item.
        /// </summary>
        /// <param name="appliedItem">The item that is applied.</param>
        void OnApplication(ILookupItem appliedItem);

        /// <summary>
        /// Maybe invoked with additional information about how the event was terminated. Invocation occurs after
        /// <see cref="OnClosed"/> and <see cref="OnApplication"/>.
        /// </summary>
        /// <param name="trigger">The kind of trigger that terminated the event</param>
        void SetTerminatedBy(IDEEvent.Trigger trigger);

        /// <summary>
        /// Returns the created event.
        /// </summary>
        /// <returns>The completion event constructed by this builder</returns>
        void OnFinished();
    }
}