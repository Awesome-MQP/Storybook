using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// When added to a class allows an old name to be specified on renaming.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public class OldName : Attribute
{
    /// <summary>
    /// The old name of this class.
    /// </summary>
    public string Name
    {
        get { return m_name; }
    }

    /// <summary>
    /// When added to a class allows an old name to be specified on renaming.
    /// </summary>
    /// <param name="name">The old name of this class.</param>
    public OldName(string name)
    {
        m_name = name;
    }

    private string m_name;
}
