﻿/* Copyright 2010-2013 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoDB.Driver.GeoJsonObjectModel.Serializers
{
    /// <summary>
    /// Represents a serializer for a GeoJsonMultiPolygonCoordinates value.
    /// </summary>
    public class GeoJsonMultiPolygonCoordinatesSerializer<TCoordinates> : BsonBaseSerializer<GeoJsonMultiPolygonCoordinates<TCoordinates>> where TCoordinates : GeoJsonCoordinates
    {
        // private fields
        private readonly IBsonSerializer<GeoJsonPolygonCoordinates<TCoordinates>> _polygonCoordinatesSerializer = BsonSerializer.LookupSerializer<GeoJsonPolygonCoordinates<TCoordinates>>();

        // public methods
        /// <summary>
        /// Deserializes a value.
        /// </summary>
        /// <param name="context">The deserialization context.</param>
        /// <returns>The value.</returns>
        public override GeoJsonMultiPolygonCoordinates<TCoordinates> Deserialize(BsonDeserializationContext context)
        {
            var bsonReader = context.Reader;

            if (bsonReader.GetCurrentBsonType() == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            }
            else
            {
                var polygons = new List<GeoJsonPolygonCoordinates<TCoordinates>>();

                bsonReader.ReadStartArray();
                while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    var polygon = _polygonCoordinatesSerializer.Deserialize(context.CreateChild(_polygonCoordinatesSerializer.ValueType));
                    polygons.Add(polygon);
                }
                bsonReader.ReadEndArray();

                return new GeoJsonMultiPolygonCoordinates<TCoordinates>(polygons);
            }
        }

        /// <summary>
        /// Serializes a value.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="value">The value.</param>
        public override void Serialize(BsonSerializationContext context, GeoJsonMultiPolygonCoordinates<TCoordinates> value)
        {
            var bsonWriter = context.Writer;

            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                var multiPolygonCoordinates = (GeoJsonMultiPolygonCoordinates<TCoordinates>)value;

                bsonWriter.WriteStartArray();
                foreach (var polygon in multiPolygonCoordinates.Polygons)
                {
                    context.SerializeWithChildContext(_polygonCoordinatesSerializer, polygon);
                }
                bsonWriter.WriteEndArray();
            }
        }
    }
}
