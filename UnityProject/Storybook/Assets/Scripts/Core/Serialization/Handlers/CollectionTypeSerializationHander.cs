using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine.Assertions;

public partial class Serializer
{
    private static void _SerializationHandler(Array target, SerializedObjectNode targetNode)
    {
        Assert.AreEqual(1, target.Length);

        int count = target.Length;
        for (int i = 0; i < count; i++)
        {
            SerializedObjectNode node = targetNode.CreateChild("entry" + i);

            object value = target.GetValue(i);

            _InvokeSerializerHandler(value, node);
        }
    }

    private static void _DeserializationHandler(out Array output, SerializedObjectNode serializedNode, Type baseType)
    {
        int length = serializedNode.GetChildCount();
        Type elementType = baseType.GetElementType();

        output = Array.CreateInstance(elementType, length);

        int i = 0;
        foreach (SerializedObjectNode serializedObjectNode in serializedNode.IterateChildren())
        {
            object element;
            _InvokeDeserializerHandler(out element, serializedObjectNode, elementType);

            output.SetValue(element, i);

            i++;
        }
    }

    private static void _SerializationHandler(IList target, SerializedObjectNode targetNode)
    {
        int count = target.Count;
        for (int i = 0; i < count; i++)
        {
            SerializedObjectNode node = targetNode.CreateChild("entry" + i);

            object value = target[i];

            _InvokeSerializerHandler(value, node);
        }
    }

    private static void _DeserializationHandler(out IList output, SerializedObjectNode serializedNode, Type baseType)
    {
        Type elementType = baseType.GetElementType();
        output = Activator.CreateInstance(baseType) as IList;

        Assert.IsNotNull(output);

        foreach (SerializedObjectNode child in serializedNode.IterateChildren())
        {
            object element = FormatterServices.GetUninitializedObject(elementType);
            _InvokeDeserializerHandler(out element, child, elementType);

            output.Add(element);
        }
    }

    private static void _SerializationHandler(IDictionary target, SerializedObjectNode targetNode)
    {
        int i = 0;
        foreach (object key in target.Keys)
        {
            SerializedObjectNode node = targetNode.CreateChild("entry" + i);
            SerializedObjectNode keyNode = node.CreateChild("key");
            SerializedObjectNode valueNode = node.CreateChild("value");

            _InvokeSerializerHandler(key, keyNode);
            _InvokeSerializerHandler(target[key], valueNode);

            i++;
        }
    }

    private static void _DeserializationHandler(out IDictionary output, SerializedObjectNode serializedNode, Type baseType)
    {
        Type[] genericArgumenTypes = baseType.GetGenericArguments();

        output = Activator.CreateInstance(baseType) as IDictionary;

        Assert.IsNotNull(output);

        if (genericArgumenTypes.Length == 0)
            genericArgumenTypes = new[] {typeof (object), typeof (object)};

        Type keyType = genericArgumenTypes[0];
        Type valueType = genericArgumenTypes[1];

        foreach (SerializedObjectNode serializedObjectNode in serializedNode.IterateChildren())
        {
            SerializedObjectNode keyNode = serializedObjectNode["key"];
            SerializedObjectNode valueNode = serializedObjectNode["value"];

            object key;
            object value;

            _InvokeDeserializerHandler(out key, keyNode, keyType);
            _InvokeDeserializerHandler(out value, valueNode, valueType);

            output.Add(key, value);
        }
    }
}
