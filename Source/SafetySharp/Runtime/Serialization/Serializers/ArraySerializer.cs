// The MIT License (MIT)
// 
// Copyright (c) 2014-2015, Institute for Software & Systems Engineering
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace SafetySharp.Runtime.Serialization.Serializers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Utilities;

	/// <summary>
	///   Serializes arrays of all types.
	/// </summary>
	internal sealed class ArraySerializer : Serializer
	{
		/// <summary>
		///   Checks whether the serialize is able to serialize the <paramref name="obj" />.
		/// </summary>
		/// <param name="obj">The obj that should be checked.</param>
		protected internal override bool CanSerialize(object obj)
		{
			return obj.GetType().IsArray;
		}

		/// <summary>
		///   Generates the state slot metadata for the <paramref name="obj" />.
		/// </summary>
		/// <param name="obj">The object the state slot metadata should be generated for.</param>
		/// <param name="objectIdentifier">The identifier of the <paramref name="obj" />.</param>
		protected internal override IEnumerable<StateSlotMetadata> GetStateSlotMetadata(object obj, int objectIdentifier)
		{
			Assert.That(((Array)obj).Rank == 1 && !obj.GetType().GetElementType().IsArray, "Multidimensional arrays are not supported.");

			yield return new StateSlotMetadata
			{
				ObjectIdentifier =  objectIdentifier,
				ObjectType = obj.GetType(),
				DataType = obj.GetType().GetElementType(),
				SlotCount = GetStateSlotCount(obj, SerializationMode.Optimized)
			};
		}

		/// <summary>
		///   Generates the code to deserialize the <paramref name="obj" />.
		/// </summary>
		/// <param name="generator">The generator that should be used to generate the code.</param>
		/// <param name="obj">The object that should be deserialized.</param>
		/// <param name="objectIdentifier">The identifier of the <paramref name="obj" />.</param>
		/// <param name="mode">The serialization mode that should be used to deserialize the object.</param>
		protected internal override void Deserialize(SerializationGenerator generator, object obj, int objectIdentifier, SerializationMode mode)
		{
			Assert.That(((Array)obj).Rank == 1 && !obj.GetType().GetElementType().IsArray, "Multidimensional arrays are not supported.");
			generator.DeserializeArray(objectIdentifier, obj.GetType().GetElementType(), ((Array)obj).GetLength(0));
		}

		/// <summary>
		///   Generates the code to serialize the <paramref name="obj" />.
		/// </summary>
		/// <param name="generator">The generator that should be used to generate the code.</param>
		/// <param name="obj">The object that should be serialized.</param>
		/// <param name="objectIdentifier">The identifier of the <paramref name="obj" />.</param>
		/// <param name="mode">The serialization mode that should be used to serialize the object.</param>
		protected internal override void Serialize(SerializationGenerator generator, object obj, int objectIdentifier, SerializationMode mode)
		{
			Assert.That(((Array)obj).Rank == 1 && !obj.GetType().GetElementType().IsArray, "Multidimensional arrays are not supported.");
			generator.SerializeArray(objectIdentifier, obj.GetType().GetElementType(), ((Array)obj).GetLength(0));
		}

		/// <summary>
		///   Gets the number of state slots required by the serialized data of <paramref name="obj" />.
		/// </summary>
		/// <param name="obj">The object consisting of state values that should be serialized.</param>
		/// <param name="mode">The serialization mode that should be used to serialize the objects.</param>
		protected internal override int GetStateSlotCount(object obj, SerializationMode mode)
		{
			return ((Array)obj).GetLength(0) * SerializationGenerator.GetStateSlotCount(obj.GetType().GetElementType());
		}

		/// <summary>
		///   Serializes the information about <paramref name="obj" />'s type using the <paramref name="writer" />.
		/// </summary>
		/// <param name="obj">The object whose type information should be serialized.</param>
		/// <param name="writer">The writer the serialized information should be written to.</param>
		protected internal override void SerializeType(object obj, BinaryWriter writer)
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			writer.Write(obj.GetType().GetElementType().AssemblyQualifiedName);
			writer.Write(((Array)obj).GetLength(0));
		}

		/// <summary>
		///   Creates an instance of the serialized type stored in the <paramref name="reader" /> without running
		///   any of the type's constructors.
		/// </summary>
		/// <param name="reader">The reader the serialized type information should be read from.</param>
		protected internal override object InstantiateType(BinaryReader reader)
		{
			var elementType = Type.GetType(reader.ReadString(), throwOnError: true);
			return Array.CreateInstance(elementType, reader.ReadInt32());
		}

		/// <summary>
		///   Gets all objects referenced by <paramref name="obj" />, excluding <paramref name="obj" /> itself.
		/// </summary>
		/// <param name="obj">The object the referenced objects should be returned for.</param>
		/// <param name="mode">The serialization mode that should be used to serialize the objects.</param>
		protected internal override IEnumerable<object> GetReferencedObjects(object obj, SerializationMode mode)
		{
			if (!obj.GetType().GetElementType().IsReferenceType())
				return Enumerable.Empty<object>();

			return (object[])obj;
		}
	}
}