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
using System.Collections;
using System.Collections.Generic;
using KaVE.JetBrains.Annotations;

namespace KaVE.VS.Statistics.Utils
{
    public interface IListing<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        ///     Deletes persisted data too
        /// </summary>
        void DeleteData();
    }

    /// <summary>
    ///     Contains a Dictionary that is loaded upon construction and saved when updated
    /// </summary>
    /// <typeparam name="TKey">Type of Keys used by the containing <see cref="Dictionary" /></typeparam>
    /// <typeparam name="TValue">Type of Values used by the containing <see cref="Dictionary" /></typeparam>
    public abstract class Listing<TKey, TValue> : IListing<TKey, TValue>
    {
        protected readonly FileHandler FileHandler;

        protected Listing(string fileName, string directoryPath)
        {
            Dictionary = new Dictionary<TKey, TValue>();

            FileHandler = new FileHandler(fileName, directoryPath);

            ReadDictionary();
        }

        [NotNull]
        protected Dictionary<TKey, TValue> Dictionary { get; private set; }

        /// <summary>
        ///     Implicitly persists the dictionary
        /// </summary>
        protected void Update(TKey key, TValue value)
        {
            if (!Dictionary.ContainsKey(key))
            {
                Dictionary.Add(key, value);
            }

            WriteDictionary();
        }

        /// <summary>
        ///     Returns default if the key is not found
        /// </summary>
        [CanBeNull]
        protected TValue GetValue(TKey key)
        {
            try
            {
                return Dictionary[key];
            }
            catch (KeyNotFoundException)
            {
                return default(TValue);
            }
        }

        /// <summary>
        ///     Deletes persisted data too
        /// </summary>
        public void DeleteData()
        {
            FileHandler.DeleteFile();
            Dictionary.Clear();
        }

        private void WriteDictionary()
        {
            FileHandler.WriteContentToFile(Dictionary);
        }

        /// <summary>
        ///     Implicitly resets the persisted data if reading failed
        /// </summary>
        private void ReadDictionary()
        {
            try
            {
                var dictionary = FileHandler.ReadContentFromFile<Dictionary<TKey, TValue>>();
                Dictionary = dictionary ?? Dictionary;
            }
            catch (Exception)
            {
                // if an Exception occured the file is corrupted and needs to be recreated
                FileHandler.ResetFile();
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}