using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Xml;
using Core.Serialization;
using UnityEngine.Assertions;

namespace Core.Serialization
{
    public partial class Serializer
    {
        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T">The type of object being serialized.</typeparam>
        /// <param name="target">The object to serialize.</param>
        /// <returns>A serialized version of the object.</returns>
        public static SerializedObject Serialize<T>(T target)
        {
            SerializedObject serializedObject = new SerializedObject();

            _InvokeSerializerHandler(target, serializedObject.RootNode);

            return serializedObject;
        }

        /// <summary>
        /// Deserializes an object.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize as.</typeparam>
        /// <param name="serializedObject">The serialized version of the object.</param>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(SerializedObject serializedObject)
        {
            object newInstance;
            _InvokeDeserializerHandler(out newInstance, serializedObject.RootNode, typeof(T));

            return (T)newInstance;
        }

        // Internal method for dynamically binding to the correct serialization handler
        private static void _InvokeSerializerHandler(object target, SerializedObjectNode targetNode)
        {
            Type targetType = target == null ? typeof(object) : target.GetType();
            Type serializerType = typeof (Serializer);

            Type[] argsTypes = {targetType, typeof (SerializedObjectNode)};
            MethodInfo handler = serializerType.GetMethod("_SerializationHandler", BindingFlags.NonPublic | BindingFlags.Static,
                Type.DefaultBinder, argsTypes, null);

            handler.Invoke(null, new[] {target, targetNode});
        }

        // Internal method for dynamically binding to the correct deserialization handler
        private static void _InvokeDeserializerHandler(out object instance, SerializedObjectNode objectNode, Type defaultType)
        {
            Type instanceType = defaultType;
            Type serializerType = typeof (Serializer);

            Type[] argsTypes = { instanceType.MakeByRefType(), typeof(SerializedObjectNode), typeof(Type) };
            MethodInfo handler = serializerType.GetMethod("_DeserializationHandler", BindingFlags.NonPublic | BindingFlags.Static,
                Type.DefaultBinder, argsTypes, null);

            object[] args = {null, objectNode, instanceType};
            handler.Invoke(null, args);

            instance = args[0];
        }
    }
     
}