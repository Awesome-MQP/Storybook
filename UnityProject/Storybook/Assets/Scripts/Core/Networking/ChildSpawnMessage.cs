using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ChildSpawnMessage : MessageBase
{
    public const short MessageId = 8000;

    public NetworkInstanceId ParentId
    {
        get { return m_parentId; }
    }

    public NetworkInstanceId Id
    {
        get { return m_id; }
    }

    public int ChildId
    {
        get { return m_childId; }
    }

    public Vector3 Position
    {
        get { return m_position; }
    }

    public byte[] Payload
    {
        get { return m_payload; }
    }

    public ChildSpawnMessage()
    {
    }

    public ChildSpawnMessage(ChildIdentity obj, byte[] payload)
    {
        NetworkIdentity childIdentity = obj.GetComponent<NetworkIdentity>();
        m_id = childIdentity.netId;

        m_childId = obj.ChildId;
        m_position = obj.transform.position;

        NetworkIdentity parentId = null;
        Transform parent = obj.transform.parent;
        while (parentId == null && parent != null)
        {
            parentId = parent.GetComponent<NetworkIdentity>();
            parent = parent.parent;
        }
        if (parentId)
            m_parentId = parentId.netId;
        else
            Debug.LogError("Error: Child identity was placed on an object with no parent with a network identity");

        m_payload = payload;
    }

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(m_id);
        writer.Write(m_parentId);
        writer.Write(m_childId);
        writer.Write(m_position);
        writer.WriteBytesFull(m_payload);
    }

    public override void Deserialize(NetworkReader reader)
    {
        m_id = reader.ReadNetworkId();
        m_parentId = reader.ReadNetworkId();
        m_childId = reader.ReadInt32();
        m_position = reader.ReadVector3();
        m_payload = reader.ReadBytesAndSize();
    }

    private NetworkInstanceId m_parentId;

    private NetworkInstanceId m_id;

    private int m_childId;

    private Vector3 m_position;

    private byte[] m_payload;
}
