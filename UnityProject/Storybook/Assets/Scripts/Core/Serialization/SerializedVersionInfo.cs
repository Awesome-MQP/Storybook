using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public struct SerializedVersionInfo
{
    /// <summary>
    /// The version number
    /// </summary>
    public int VersionNumber
    {
        get { return m_versionNumber; }
    }

    /// <summary>
    /// The type name of the version info.
    /// </summary>
    public string TypeName
    {
        get { return m_typeName; }
    }

    /// <summary>
    /// Parse the version info from a string
    /// </summary>
    /// <param name="versionString">The full version string</param>
    /// <returns>The new version object</returns>
    public static SerializedVersionInfo Parse(string versionString)
    {
        SerializedVersionInfo versionInfo = new SerializedVersionInfo();

        int equalSignIndex = versionString.IndexOf('=');
        Assert.IsTrue(equalSignIndex != -1);

        versionInfo.m_typeName = versionString.Substring(0, equalSignIndex);
        versionInfo.m_versionNumber = int.Parse(versionString.Substring(equalSignIndex + 1));

        return versionInfo;
    }

    /// <summary>
    /// Create version info from the type info
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static SerializedVersionInfo Create(Type type)
    {
        if (type.IsDefined(typeof (VersionAttribute), false))
        {
            VersionAttribute versionInfo = (VersionAttribute) type.GetCustomAttributes(typeof (VersionAttribute), false)[0];

            string typeName = type.FullName;
            int versionNumber = versionInfo.VersionNumber;

            return new SerializedVersionInfo(typeName, versionNumber);
        }
        else
        {
            string typeName = type.FullName;
            const int versionNumber = 0;

            return new SerializedVersionInfo(typeName, versionNumber);
        }
    }

    private SerializedVersionInfo(string typeName, int versionNumber)
    {
        m_typeName = typeName;
        m_versionNumber = versionNumber;
    }

    public override string ToString()
    {
        string baseString = string.Format("{0}={1}", m_typeName, m_versionNumber);

        return baseString;
    }

    private int m_versionNumber;

    private string m_typeName;
}
