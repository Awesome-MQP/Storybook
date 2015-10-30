using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

namespace Core.Serialization
{
    public partial class Serializer
    {
        private static void _SerializationHandler(object target, SerializedObjectNode targetNode)
        {
            Type targetType = target.GetType();
            FieldInfo[] fields = TypeHelper.GetFields(targetType, typeof (SerializeField), true);

            foreach (FieldInfo fieldInfo in fields)
            {
                SerializedObjectNode fieldNode = targetNode.CreateChild(fieldInfo.Name);
                object fieldValue = fieldInfo.GetValue(target);

                _InvokeSerializerHandler(fieldValue, fieldNode);
            }

            targetNode.SetVersionInfo(targetType);
        }

        private static void _DeserializationHandler(out object output, SerializedObjectNode serializedNode, Type baseType)
        {
            if (serializedNode.Value == "NULL")
            {
                output = null;
                return;
            }

            Type type = baseType;

            output = Activator.CreateInstance(type);

            //apply version updating
            Type expectedType = type;
            foreach (SerializedVersionInfo serializedVersionInfo in serializedNode.IterateVersionInfo())
            {
                Type savedType = TypeHelper.GetType(serializedVersionInfo.TypeName);
                if (savedType == expectedType || savedType == null)
                {
                    SerializedVersionInfo currentVersionInfo = SerializedVersionInfo.Create(expectedType);

                    for (int i = serializedVersionInfo.VersionNumber + 1; i <= currentVersionInfo.VersionNumber; i++)
                    {
                        MethodInfo upgradeMethodInfo = expectedType.GetMethod(string.Format("_UpgradeToVersion{0}", i),
                            BindingFlags.Static | BindingFlags.NonPublic, null, new[] {typeof(SerializedObjectNode)}, null);

                        if (upgradeMethodInfo != null)
                        {
                            foreach (SerializedObjectNode childNode in serializedNode.IterateChildren())
                            {
                                upgradeMethodInfo.Invoke(null, new object[] {childNode});
                            }
                        }
                    }
                }

                if (expectedType != null)
                    expectedType = expectedType.BaseType;
            }

            foreach (SerializedObjectNode childNode in serializedNode.IterateChildren())
            {
                FieldInfo field = TypeHelper.GetField(type, childNode.Name);
                if (field == null)
                    continue;

                Type fieldType = field.FieldType;

                object fieldObject;
                _InvokeDeserializerHandler(out fieldObject, childNode, fieldType);

                field.SetValue(output, fieldObject);
            }
        }
    }
     
}