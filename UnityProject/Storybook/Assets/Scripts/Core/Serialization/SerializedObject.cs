using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine.Assertions;

/// <summary>
/// Represents the serialized version of an object.
/// </summary>
public class SerializedObject
{
    /// <summary>
    /// The root node of this object.
    /// </summary>
    public SerializedObjectNode RootNode
    {
        get { return m_rootNode; }
    }

    /// <summary>
    /// Creates a new serialized object to serialize into.
    /// </summary>
    public SerializedObject()
    {
        XmlDocument document = new XmlDocument();
        m_rootXmlNode = document.CreateElement("root");
        document.AppendChild(m_rootXmlNode);

        m_rootNode = new SerializedObjectNode(m_rootXmlNode);
    }

    /// <summary>
    /// Creates a serialized object.
    /// </summary>
    /// <param name="document">The XmlDocument to read from.</param>
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

    /// <summary>
    /// Creates a serialized object.
    /// </summary>
    /// <param name="file">The file to read the serialized object from.</param>
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

    /// <summary>
    /// Creates a serialized object.
    /// </summary>
    /// <param name="stream">The stream to read the serialized object data from.</param>
    public SerializedObject(Stream stream)
    {
        XmlReader reader = XmlReader.Create(stream);
        XmlDocument document = new XmlDocument();
        document.Load(reader);

        m_rootXmlNode = document["root"];
        m_rootNode = new SerializedObjectNode(m_rootXmlNode);
    }

    /// <summary>
    /// Writes a serialized object to a file.
    /// </summary>
    /// <param name="file">The name of the file to write to</param>
    public void WriteTo(string file)
    {
        using (FileStream fileStream = new FileStream(file, FileMode.OpenOrCreate))
        {
            WriteTo(fileStream);
        }
    }

    /// <summary>
    /// Writes a serialized object to a stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
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
