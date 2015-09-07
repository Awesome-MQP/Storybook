using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class TypeHelper
{
    [RuntimeInitializeOnLoadMethod]
    private static void Startup()
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

    public static Type GetType(string name)
    {
        if (oldNameLookupTable.ContainsKey(name))
            name = oldNameLookupTable[name];

        Type foundType;
        typeLookupTable.TryGetValue(name, out foundType);

        return foundType;
    }

    public static string GetRealTypeName(string name)
    {
        if (oldNameLookupTable.ContainsKey(name))
            name = oldNameLookupTable[name];

        return name;
    }

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

    public static FieldInfo[] GetFields(Type t)
    {
        return GetFields(t, null, false);
    }

    public static FieldInfo[] GetFields(Type t, Type requiredAttributeType, bool inherited)
    {
        HashSet<FieldInfo> fieldsSet = new HashSet<FieldInfo>();

        FieldInfo[] publicFields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo publicField in publicFields)
        {
            if(requiredAttributeType == null || publicField.IsDefined(requiredAttributeType, inherited))
                fieldsSet.Add(publicField);
        }

        AppendPrivateFields(t, fieldsSet, requiredAttributeType, inherited);

        FieldInfo[] fields = new FieldInfo[fieldsSet.Count];
        int i = 0;
        foreach (FieldInfo fieldInfo in fieldsSet)
        {
            fields[i] = fieldInfo;
            i++;
        }

        return fields;
    }

    private static void AppendPrivateFields(Type t, HashSet<FieldInfo> fieldList, Type requiredAttributeType, bool inherited)
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
