using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Loader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnPreLoad()
    {
        AssemblyName[] referencAssemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
        AssemblyName[] allAssemblyNames = new AssemblyName[referencAssemblyNames.Length + 1];

        referencAssemblyNames.CopyTo(allAssemblyNames, 1);
        allAssemblyNames[0] = Assembly.GetExecutingAssembly().GetName();

        List<MethodInfo> startMethods = new List<MethodInfo>();

        foreach (AssemblyName assemblyName in allAssemblyNames)
        {
            Assembly assembly = Assembly.Load(assemblyName);

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                foreach (MethodInfo method in methods)
                {
                    if (method.IsDefined(typeof (PreLoadMethod), true))
                    {
                        startMethods.Add(method);
                    }
                }
            }
        }

        startMethods.Sort((infoA, infoB) =>
        {
            PreLoadMethod a = infoA.GetCustomAttributes(typeof (PreLoadMethod), true)[0] as PreLoadMethod;
            PreLoadMethod b = infoA.GetCustomAttributes(typeof(PreLoadMethod), true)[0] as PreLoadMethod;

            return a.Order - b.Order;
        });

        foreach (MethodInfo startMethod in startMethods)
        {
            startMethod.Invoke(false, null);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnPostLoad()
    {
        AssemblyName[] referencAssemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
        AssemblyName[] allAssemblyNames = new AssemblyName[referencAssemblyNames.Length + 1];

        referencAssemblyNames.CopyTo(allAssemblyNames, 1);
        allAssemblyNames[0] = Assembly.GetExecutingAssembly().GetName();

        List<MethodInfo> startMethods = new List<MethodInfo>();

        foreach (AssemblyName assemblyName in allAssemblyNames)
        {
            Assembly assembly = Assembly.Load(assemblyName);

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

                foreach (MethodInfo method in methods)
                {
                    if (method.IsDefined(typeof(PostLoadMethod), true))
                    {
                        startMethods.Add(method);
                    }
                }
            }
        }

        startMethods.Sort((infoA, infoB) =>
        {
            PostLoadMethod a = infoA.GetCustomAttributes(typeof(PostLoadMethod), true)[0] as PostLoadMethod;
            PostLoadMethod b = infoA.GetCustomAttributes(typeof(PostLoadMethod), true)[0] as PostLoadMethod;

            return a.Order - b.Order;
        });

        foreach (MethodInfo startMethod in startMethods)
        {
            startMethod.Invoke(false, null);
        }
    }
}

public class PreLoadMethod : Attribute
{
    public int Order
    {
        get { return m_order; }
        set { m_order = value; }
    }

    private int m_order = 1000;
}

public class PostLoadMethod : Attribute
{
    public int Order
    {
        get { return m_order; }
        set { m_order = value; }
    }

    private int m_order = 1000;
}