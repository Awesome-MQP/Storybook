using System;
using UnityEngine;
using System.Collections;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class OldName : Attribute
{
    public string Name
    {
        get { return m_name; }
    }

    public OldName(string name)
    {
        m_name = name;
    }

    private string m_name;
}
