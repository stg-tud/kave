/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

namespace KaVE.Commons.Utils
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
        /// Gets the reference's target object.
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

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(WeakReference<TRef> other)
        {
            return IsAlive() == other.IsAlive() && Equals(Target, other.Target);
        }

        public override int GetHashCode()
        {
            return IsAlive() ? Target.GetHashCode() : 1;
        }
    }
}
