using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine.Assertions;

public class SerializedObject
{
    public SerializedObjectNode RootNode
    {
        get { return m_rootNode; }
    }

    public SerializedObject()
    {
        XmlDocument document = new XmlDocument();
        m_rootXmlNode = document.CreateElement("root");
        document.AppendChild(m_rootXmlNode);

        m_rootNode = new SerializedObjectNode(m_rootXmlNode);
    }

    public SerializedObject(XmlDocument document)
    {
        m_rootXmlNode = document["root"];
        if (m_rootXmlNode == null)
        {
            m_rootXmlNode = document.CreateElement("root");
            document.AppendChild(m_rootXmlNode);
        }

        m_rootNode = new SerializedObjectNode(m_rootXmlNode);
    }

    public SerializedObject(string file)
    {
        using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
        {
            XmlReader reader = XmlReader.Create(stream);
            XmlDocument document = new XmlDocument();
            document.Load(reader);

            m_rootXmlNode = document["root"];
            m_rootNode = new SerializedObjectNode(m_rootXmlNode);
        }
    }

    public SerializedObject(Stream stream)
    {
        XmlReader reader = XmlReader.Create(stream);
        XmlDocument document = new XmlDocument();
        document.Load(reader);

        m_rootXmlNode = document["root"];
        m_rootNode = new SerializedObjectNode(m_rootXmlNode);
    }

    public void WriteTo(string file)
    {
        using (FileStream fileStream = new FileStream(file, FileMode.OpenOrCreate))
        {
            WriteTo(fileStream);
        }
    }

    public void WriteTo(Stream stream)
    {
        XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Unicode);
        writer.Formatting = Formatting.Indented;
        m_rootXmlNode.OwnerDocument.WriteTo(writer);
        writer.Flush();
    }

    private SerializedObjectNode m_rootNode;
    private XmlNode m_rootXmlNode;
}
