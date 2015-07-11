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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace KaVE.FeedbackProcessor.Database
{
    internal class KaVECollectionSerializer<T> : EnumerableSerializerBase<T>
    {
        public KaVECollectionSerializer() : base(new ArraySerializationOptions()) {}

        protected override void AddItem(object instance, T item)
        {
            ((ICollection<T>) instance).Add(item);
        }

        protected override object CreateInstance(Type actualType)
        {
            if (actualType.IsInterface)
            {
                if (actualType == typeof (IKaVESet<T>))
                {
                    return new KaVEHashSet<T>();
                }
                if (actualType == typeof (IKaVEList<T>))
                {
                    return new KaVEList<T>();
                }
            }

            throw new BsonSerializationException(string.Format("no KaVE collection: {0}", actualType));
        }

        protected override IEnumerable<T> EnumerateItemsInSerializationOrder(object instance)
        {
            return (IEnumerable<T>) instance;
        }

        protected override object FinalizeResult(object instance, Type actualType)
        {
            return instance;
        }
    }

    class KaVECollectionSerializer : EnumerableSerializerBase
    {
        public KaVECollectionSerializer() : base(new ArraySerializationOptions()) { }

        protected override void AddItem(object instance, object item)
        {
            var collection = instance as IProposalCollection;
            if (collection != null)
            {
                Asserts.That(item is IProposal);
                collection.Add((IProposal) item);
            }
            else
            {
                throw new BsonSerializationException(string.Format("no KaVE collection: {0}", instance.GetType()));
            }
        }

        protected override object CreateInstance(Type actualType)
        {
            if (actualType.IsInterface)
            {
                if (actualType == typeof(IProposalCollection))
                {
                    return new ProposalCollection();
                }
            }

            throw new BsonSerializationException(string.Format("no KaVE collection: {0}", actualType));
        }

        protected override IEnumerable EnumerateItemsInSerializationOrder(object instance)
        {
            return (IEnumerable)instance;
        }

        protected override object FinalizeResult(object instance, Type actualType)
        {
            return instance;
        }
    }
}