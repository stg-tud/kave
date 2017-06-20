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
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace KaVE.FeedbackProcessor.Database
{
    public class DateTimeTicksSerializer : DateTimeSerializer
    {
        public override object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
        {
            var obj = base.Deserialize(bsonReader, nominalType, options);
            var dt = (DateTime)obj;
            return new DateTime(dt.Ticks);
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
        {
            var obj = base.Deserialize(bsonReader, nominalType, actualType, options);
            var dt = (DateTime)obj;
            return new DateTime(dt.Ticks);
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            var dt = (DateTime)value;
            bsonWriter.WriteInt64(dt.Ticks);
        }
    }
}
