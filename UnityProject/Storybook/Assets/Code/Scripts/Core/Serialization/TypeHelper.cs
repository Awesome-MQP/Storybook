using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class TypeHelper
{
    // Type init script
    [PreLoadMethod(Order = -1000)]
    private static void _Startup()
    {
        AssemblyName[] referencAssemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
        AssemblyName[] allAssemblyNames = new AssemblyName[referencAssemblyNames.Length + 1];

        referencAssemblyNames.CopyTo(allAssemblyNames, 1);
        allAssemblyNames[0] = Assembly.GetExecutingAssembly().GetName();

        foreach (AssemblyName assemblyName in allAssemblyNames)
        {
            Assembly assembly = Assembly.Load(assemblyName);

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if(typeLookupTable.ContainsKey(type.FullName))
                    continue;
                
                typeLookupTable.Add(type.FullName, type);

                if (type.IsDefined(typeof (OldName), false))
                {
                    OldName oldNameInfo = (OldName) type.GetCustomAttributes(typeof (OldName), false)[0];
                    oldNameLookupTable.Add(oldNameInfo.Name, type.FullName);
                }
            }
        }
    }

#if UNITY_EDITOR
    static TypeHelper()
    {
        _Startup();
    }
#endif

    /// <summary>
    /// Search for a type by name.
    /// </summary>
    /// <param name="name">The full name of the type to look for.</param>
    /// <returns>The reflected type or null if the type could not be found.</returns>
    public static Type GetType(string name)
    {
        if (oldNameLookupTable.ContainsKey(name))
            name = oldNameLookupTable[name];

        Type foundType;
        typeLookupTable.TryGetValue(name, out foundType);

        return foundType;
    }

    /// <summary>
    /// Gets the real name of a type. This is useful if a type name may be the old type name of a type.
    /// </summary>
    /// <param name="name">The name to get the real name of.</param>
    /// <returns>The real name of a type, or null if the type name does not exist.</returns>
    public static string GetRealTypeName(string name)
    {
        if (oldNameLookupTable.ContainsKey(name))
            name = oldNameLookupTable[name];
        else if (typeLookupTable.ContainsKey(name))
            return name;

        return null;
    }

    /// <summary>
    /// Find a field by name in a type.
    /// </summary>
    /// <param name="t">The type to look in.</param>
    /// <param name="fieldName">The field name.</param>
    /// <returns>The reflected field info, or null if not found.</returns>
    public static FieldInfo GetField(Type t, string fieldName)
    {
        while (true)
        {
            FieldInfo publicFieldInfo = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            if (publicFieldInfo != null)
                return publicFieldInfo;

            FieldInfo privateFieldInfo = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (privateFieldInfo != null)
                return privateFieldInfo;

            if (t.BaseType != null)
            {
                t = t.BaseType;
                continue;
            }

            return null;
        }
    }

    /// <summary>
    /// Gets all fields from a type.
    /// </summary>
    /// <param name="t">The reflected type to get all fields from.</param>
    /// <returns>An array of all fields in this type.</returns>
    public static FieldInfo[] GetFields(Type t)
    {
        return GetFields(t, null, false);
    }

    /// <summary>
    /// Gets all fields with an attribute in type.
    /// </summary>
    /// <param name="t">The reflected type to get fields from.</param>
    /// <param name="requiredAttributeType">The required type of attribute to have on a field.</param>
    /// <param name="inherited">Should the attribute be inherited or not.</param>
    /// <returns>An array of all fields that matched the input rules.</returns>
    public static FieldInfo[] GetFields(Type t, Type requiredAttributeType, bool inherited)
    {
        HashSet<FieldInfo> fieldsSet = new HashSet<FieldInfo>();

        FieldInfo[] publicFields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo publicField in publicFields)
        {
            if(requiredAttributeType == null || publicField.IsDefined(requiredAttributeType, inherited))
                fieldsSet.Add(publicField);
        }

        _AppendPrivateFields(t, fieldsSet, requiredAttributeType, inherited);

        FieldInfo[] fields = new FieldInfo[fieldsSet.Count];
        int i = 0;
        foreach (FieldInfo fieldInfo in fieldsSet)
        {
            fields[i] = fieldInfo;
            i++;
        }

        return fields;
    }

    private static void _AppendPrivateFields(Type t, HashSet<FieldInfo> fieldList, Type requiredAttributeType, bool inherited)
    {
        while (true)
        {
            FieldInfo[] privateFieldInfos = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo privateFieldInfo in privateFieldInfos)
            {
                if (!fieldList.Contains(privateFieldInfo) && (requiredAttributeType == null || privateFieldInfo.IsDefined(requiredAttributeType, inherited)))
                    fieldList.Add(privateFieldInfo);
            }

            if (t.BaseType != null)
            {
                t = t.BaseType;
                continue;
            }
            break;
        }
    }

    private static Dictionary<string, Type> typeLookupTable = new Dictionary<string, Type>(300);
    private static Dictionary<string, string> oldNameLookupTable = new Dictionary<string, string>();
}
