using System;

/// <summary>
/// Specifies the version of a class or struct
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class VersionAttribute : Attribute
{
    public int VersionNumber
    {
        get { return m_versionNumber; }
    }

    public VersionAttribute(int versionNumber)
    {
        m_versionNumber = versionNumber;
    }


    private int m_versionNumber;
}
