using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine.Assertions;

public class SerializedObjectNode
{
    /// <summary>
    /// The version info of this node.
    /// </summary>
    public SerializedVersionInfo MainVersionInfo
    {
        get
        {
            if (m_versionInfo.Count > 0)
                return m_versionInfo.First.Value;
            return new SerializedVersionInfo();
        }
    }

    /// <summary>
    /// The nodes name.
    /// </summary>
    public string Name
    {
        get { return m_xmlNode.Name; }
    }

    /// <summary>
    /// The value stored at this node.
    /// </summary>
    public string Value
    {
        get { return m_xmlNode.InnerText; }
        set
        {
            Assert.IsTrue(m_xmlNode.ChildNodes.Count == 0);
            m_xmlNode.InnerText = value;
        }
    }

    /// <summary>
    /// The parent node 
    /// </summary>
    public SerializedObjectNode ParentNode
    {
        get { return m_parentNode; }
    }

    /// <summary>
    /// Get the child element of this node
    /// </summary>
    /// <param name="name">The full name path of the child</param>
    /// <returns>The child node at the full path</returns>
    public SerializedObjectNode this[string name]
    {
        get
        {
            if (string.IsNullOrEmpty(name))
                return this;

            int delemiterIndex = name.IndexOf('/');
            string childName;
            if (delemiterIndex > -1)
                childName = name.Substring(0, delemiterIndex);
            else
                childName = name;

            string remainderName = name.Substring(delemiterIndex + 1);

            return m_children[childName][remainderName];
        }
    }

    /// <summary>
    /// Creates a serialized object from an XmlNode that is the child of a parent node
    /// </summary>
    /// <param name="node">The xml node that this serialized object is wrapping</param>
    /// <param name="parentNode">The parent node that we are a part of</param>
    protected SerializedObjectNode(XmlNode node, SerializedObjectNode parentNode)
    {
        m_xmlNode = node;
        m_parentNode = parentNode;

        if (m_xmlNode.Attributes != null)
        {
            XmlAttribute versionAttribute = m_xmlNode.Attributes["version"];

            if (versionAttribute != null)
            {
                string fullVersionString = versionAttribute.Value;
                while (!string.IsNullOrEmpty(fullVersionString))
                {
                    int versionNameDelimiterIndex = fullVersionString.IndexOf(',');
                    string versionName = versionNameDelimiterIndex  < 0 ? fullVersionString : fullVersionString.Substring(0, versionNameDelimiterIndex);

                    if (versionNameDelimiterIndex > -1)
                        fullVersionString = fullVersionString.Substring(versionNameDelimiterIndex + 1);
                    else
                        fullVersionString = null;

                    SerializedVersionInfo newVersionInfo = SerializedVersionInfo.Parse(versionName);
                    m_versionInfo.AddFirst(newVersionInfo);
                }
            }
        }

        foreach (XmlNode childNode in m_xmlNode.ChildNodes)
        {
            SerializedObjectNode newChildNode = new SerializedObjectNode(childNode, this);
            m_children.Add(childNode.Name, newChildNode);
        }
    }

    /// <summary>
    /// Creates a serialized object wrapper around an xml node
    /// </summary>
    /// <param name="node">The xml node that we are serializing around</param>
    public SerializedObjectNode(XmlNode node) : this(node, null)
    {
    }

    /// <summary>
    /// Get the number of classes that have version info
    /// </summary>
    /// <returns></returns>
    public int GetVersionClassesCount()
    {
        return m_versionInfo.Count;
    }

    public SerializedVersionInfo GetVersionInfo(int classIndex)
    {
        Assert.IsTrue(classIndex < m_versionInfo.Count);

        LinkedListNode<SerializedVersionInfo> node = m_versionInfo.First;
        for (int i = 0; i < classIndex; i++)
        {
            node = node.Next;
        }

        return node.Value;
    }

    public void SetVersionInfo(Type type)
    {
        m_versionInfo.Clear();

        int versionDepthCount = 0;
        while (type != null)
        {
            SerializedVersionInfo serializedVersionInfo = SerializedVersionInfo.Create(type);
            if (serializedVersionInfo.VersionNumber == 0 && versionDepthCount != 0)
                break;

            m_versionInfo.AddFirst(serializedVersionInfo);

            type = type.BaseType;
            versionDepthCount++;
        }

        if (m_versionInfo.Count == 0)
        {
            m_xmlNode.Attributes.RemoveNamedItem("version");
        }
        else
        {
            XmlAttribute versionAttribute = m_xmlNode.OwnerDocument.CreateAttribute("version");
            foreach (SerializedVersionInfo serializedVersionInfo in m_versionInfo)
            {
                versionAttribute.Value = string.Format(",{0}", serializedVersionInfo) + versionAttribute.Value;
            }

            versionAttribute.Value = versionAttribute.Value.Substring(1, versionAttribute.Value.Length - 1);
            m_xmlNode.Attributes.Append(versionAttribute);
        }
    }

    public IEnumerable<SerializedVersionInfo> IterateVersionInfo()
    {
        return m_versionInfo;
    }

    public IEnumerable<SerializedObjectNode> IterateChildren()
    {
        return m_children.Values;
    }

    public int GetChildCount()
    {
        return m_children.Count;
    }

    public SerializedObjectNode CreateChild(string name)
    {
        return CreateChild(name, null);
    }

    public SerializedObjectNode CreateChild(string name, string versionString)
    {
        XmlNode xmlChildNode = m_xmlNode.OwnerDocument.CreateElement(name);
        m_xmlNode.AppendChild(xmlChildNode);

        if(!string.IsNullOrEmpty(versionString))
        {
            XmlAttribute versionAttribute = m_xmlNode.OwnerDocument.CreateAttribute("Version");
            versionAttribute.Value = versionString;
            xmlChildNode.Attributes.Append(versionAttribute);
        }

        SerializedObjectNode chidlNode = new SerializedObjectNode(xmlChildNode, this);
        m_children.Add(name, chidlNode);

        return chidlNode;
    }

    public bool RemoveChild(string name)
    {
        if (!m_children.ContainsKey(name))
            return false;

        SerializedObjectNode childNode = m_children[name];
        m_xmlNode.RemoveChild(childNode.m_xmlNode);

        m_children.Remove(name);

        return true;
    }

    public void RemoveThis()
    {
        Assert.IsNotNull(m_parentNode);

        m_parentNode.RemoveChild(Name);
    }

    public void Rename(string newName)
    {
        XmlNode newNode = m_xmlNode.OwnerDocument.CreateElement(newName);
        m_xmlNode.ParentNode.AppendChild(newNode);

        foreach (XmlNode childNode in m_xmlNode.ChildNodes)
        {
            newNode.AppendChild(childNode);
        }
        foreach (XmlAttribute attribute in m_xmlNode.Attributes)
        {
            newNode.Attributes.Append(attribute);
        }

        m_xmlNode.ParentNode.RemoveChild(m_xmlNode);
        m_xmlNode = newNode;
    }

    private XmlNode m_xmlNode;

    private LinkedList<SerializedVersionInfo> m_versionInfo = new LinkedList<SerializedVersionInfo>();

    private SerializedObjectNode m_parentNode;

    //All child nodes to this node
    private Dictionary<string, SerializedObjectNode> m_children = new Dictionary<string, SerializedObjectNode>();
}
