using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// A new spawn message for spawning a child over the network.
/// </summary>
public class ChildSpawnMessage : MessageBase
{
    /// <summary>
    /// THe message id for this message
    /// </summary>
    public const short MessageId = 8000;

    /// <summary>
    /// The id for the parent to spawn under
    /// </summary>
    public NetworkInstanceId ParentId
    {
        get { return m_parentId; }
    }

    /// <summary>
    /// The network id to assign the child
    /// </summary>
    public NetworkInstanceId Id
    {
        get { return m_id; }
    }

    /// <summary>
    /// The id of the child to spawn
    /// </summary>
    public int ChildId
    {
        get { return m_childId; }
    }

    /// <summary>
    /// The position to spawn the child at
    /// </summary>
    public Vector3 Position
    {
        get { return m_position; }
    }

    /// <summary>
    /// Payload (idk it was in th eoriginal one)
    /// </summary>
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
