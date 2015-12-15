using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using UnityEngine;


[Serializable]
public sealed class ResourceAsset
{
    private Type m_typeCache;

    [SerializeField] private string m_asset = "";

    [SerializeField] private string m_baseTypeName = typeof (UnityEngine.Object).FullName;

#if UNITY_EDITOR
    //Only used in the editor to find the asset.
    [SerializeField] private string m_editorAsset = "";
#endif

    public Type BaseType
    {
        get
        {
            if (m_typeCache != null)
                return m_typeCache;

            m_typeCache = TypeHelper.GetType(m_baseTypeName);
            return m_typeCache;
        }
    }

    public string AssetName
    {
        get { return m_baseTypeName; }
    }

    public ResourceAsset()
    { }

    public ResourceAsset(Type baseType)
    {
        m_baseTypeName = baseType.FullName;
        m_typeCache = baseType;
    }

    public ResourceAsset(string assetName, Type baseType)
    {
        m_asset = assetName;
        m_baseTypeName = baseType.FullName;
        m_typeCache = baseType;
    }

    public T GetAsset<T>() where T : UnityEngine.Object
    {
        return Resources.Load<T>(m_asset);
    }
}
