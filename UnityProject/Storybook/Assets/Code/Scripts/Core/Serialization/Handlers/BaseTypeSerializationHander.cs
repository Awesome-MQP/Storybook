﻿using System;
using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Xml;
using UnityEngine.Assertions;

namespace Core.Serialization
{

    public partial class Serializer
    {
        private static void _SerializationHandler(char target, SerializedObjectNode targetNode)
        {
            targetNode.Value = target.ToString();
        }

        private static void _DeserializationHandler(out char output, SerializedObjectNode serializedNode, Type baseType)
        {
            Assert.AreEqual(1, serializedNode.Value.Length);
            output = serializedNode.Value[0];
        }

        private static void _SerializationHandler(int target, SerializedObjectNode targetNode)
        {
            targetNode.Value = target.ToString();
        }

        private static void _DeserializationHandler(out int output, SerializedObjectNode serializedNode, Type baseType)
        {
            output = int.Parse(serializedNode.Value);
        }

        private static void _SerializationHandler(float target, SerializedObjectNode targetNode)
        {
            targetNode.Value = target.ToString(CultureInfo.InvariantCulture);
        }

        private static void _DeserializationHandler(out float output, SerializedObjectNode serializedNode, Type baseType)
        {
            output = float.Parse(serializedNode.Value);
        }

        private static void _SerializationHandler(string target, SerializedObjectNode targetNode)
        {
            targetNode.Value = target;
        }

        private static void _DeserializationHandler(out string output, SerializedObjectNode serializedNode, Type baseType)
        {
            output = serializedNode.Value;
        }

        private static void _SerializationHandler(DateTime target, SerializedObjectNode targetNode)
        {
            targetNode.Value = target.ToString(CultureInfo.InvariantCulture);
        }

        private static void _DeserializationHandler(out DateTime output, SerializedObjectNode serializedNode, Type baseType)
        {
            output = DateTime.Parse(serializedNode.Value);
        }

        private static void _SerializationHandler(TimeSpan target, SerializedObjectNode targetNode)
        {
            targetNode.Value = target.ToString();
        }

        private static void _DeserializationHandler(out TimeSpan output, SerializedObjectNode serializedNode, Type baseType)
        {
            output = TimeSpan.Parse(serializedNode.Value);
        }

        private static void _SerializationHandler(Vector2 target, SerializedObjectNode targetNode)
        {
            targetNode.Value = string.Format("{0} {1}", target.x, target.y);
        }

        private static void _DeserializationHandler(out Vector2 output, SerializedObjectNode serializedNode, Type baseType)
        {
            string[] parts = serializedNode.Value.Split(' ');

            output.x = float.Parse(parts[0]);
            output.y = float.Parse(parts[1]);
        }

        private static void _SerializationHandler(Vector3 target, SerializedObjectNode targetNode)
        {
            targetNode.Value = string.Format("{0} {1} {2}", target.x, target.y, target.z);
        }

        private static void _DeserializationHandler(out Vector3 output, SerializedObjectNode serializedNode, Type baseType)
        {
            string[] parts = serializedNode.Value.Split(' ');

            output.x = float.Parse(parts[0]);
            output.y = float.Parse(parts[1]);
            output.z = float.Parse(parts[2]);
        }

        private static void _SerializationHandler(Quaternion target, SerializedObjectNode targetNode)
        {
            targetNode.Value = string.Format("{0} {1} {2} {3}", target.x, target.y, target.z, target.w);
        }

        private static void _DeserializationHandler(out Quaternion output, SerializedObjectNode serializedNode, Type baseType)
        {
            string[] parts = serializedNode.Value.Split(' ');

            output.x = float.Parse(parts[0]);
            output.y = float.Parse(parts[1]);
            output.z = float.Parse(parts[2]);
            output.w = float.Parse(parts[3]);
        }
    }
     
}