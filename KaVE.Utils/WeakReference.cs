using System;

namespace KaVE.Utils
{
    /// <summary>
    /// A type-safe wrapper around <see cref="System.WeakReference"/>.
    /// </summary>
    /// <typeparam name="TRef"></typeparam>
    public struct WeakReference<TRef> where TRef : class
    {
        private readonly WeakReference _reference;

        /// <param name="reference">the instance to create a weak reference for</param>
        /// <param name="trackResurrection">whether or not to track object resurrection. Default is false</param>
        public WeakReference(TRef reference, bool trackResurrection = false)
        {
            _reference = new WeakReference(reference, trackResurrection);
        }
        
        /// <summary>
        /// Gets or sets the reference's target object.
        /// </summary>
        public TRef Target
        {
            get
            {
                return (TRef) _reference.Target;
            }

            set
            {
                _reference.Target = value;
            }
        }

        /// <summary>
        /// Indicates wheather the object referenced by this reference has been garbage collected.
        /// </summary>
        public bool IsAlive()
        {
            return _reference.IsAlive;
        }
    }
}
