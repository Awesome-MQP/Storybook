using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Xml;
using UnityEngine.Assertions;

public partial class Serializer
{
    public static SerializedObject Serialize<T>(T target)
    {
        SerializedObject serializedObject = new SerializedObject();

        InvokeSerializerHandler(target, serializedObject.RootNode);

        return serializedObject;
    }

    public static T Deserialize<T>(SerializedObject serializedObject)
    {
        object newInstance;
        InvokeDeserializerHandler(out newInstance, serializedObject.RootNode, typeof(T));

        return (T)newInstance;
    }

    private static void InvokeSerializerHandler(object target, SerializedObjectNode targetNode)
    {
        Type targetType = target == null ? typeof(object) : target.GetType();
        Type serializerType = typeof (Serializer);

        Type[] argsTypes = {targetType, typeof (SerializedObjectNode)};
        MethodInfo handler = serializerType.GetMethod("SerializationHandler", BindingFlags.NonPublic | BindingFlags.Static,
            Type.DefaultBinder, argsTypes, null);

        handler.Invoke(null, new[] {target, targetNode});
    }

    private static void InvokeDeserializerHandler(out object instance, SerializedObjectNode objectNode, Type defaultType)
    {
        Type instanceType = defaultType;
        Type serializerType = typeof (Serializer);

        Type[] argsTypes = { instanceType.MakeByRefType(), typeof(SerializedObjectNode), typeof(Type) };
        MethodInfo handler = serializerType.GetMethod("DeserializationHandler", BindingFlags.NonPublic | BindingFlags.Static,
            Type.DefaultBinder, argsTypes, null);

        object[] args = {null, objectNode, instanceType};
        handler.Invoke(null, args);

        instance = args[0];
    }
}
