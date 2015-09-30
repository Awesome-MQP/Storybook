using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.Assertions;
using UnityEngine.Networking;

public class AdvanceNetworkManager : NetworkManager
{
    void Awake()
    {
        NetworkServer.SetNetworkConnectionClass<AdvanceNetworkConnection>();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        conn.RegisterHandler(ChildSpawnMessage.MessageId, SpawnChildMessageHandler);
        base.OnClientConnect(conn);
    }

    private void SpawnChildMessageHandler(NetworkMessage netMsg)
    {
        ChildSpawnMessage message = null;
        try
        {
            message = netMsg.ReadMessage<ChildSpawnMessage>();
        }
        catch (Exception e)
        {
            Debug.LogError("Could not parse message.");
            throw;
        }
        

        GameObject parent = ClientScene.FindLocalObject(message.ParentId);
        Assert.IsNotNull(parent, "Parent not found.");

        ChildIdentity[] childrenIdentities = parent.GetComponentsInChildren<ChildIdentity>();
        int childId = message.ChildId;

        ChildIdentity child = null;
        foreach (ChildIdentity identity in childrenIdentities)
        {
            if (identity.ChildId == childId)
            {
                child = identity;
                break;
            }
        }

        Assert.IsNotNull(child, string.Format("Child of id {0} not found in object {1}.", childId, parent));

        NetworkIdentity childIdentity = child.GetComponent<NetworkIdentity>();
        typeof (ClientScene).GetMethod("ApplySpawnPayload", BindingFlags.NonPublic | BindingFlags.Static)
            .Invoke(null, new object[] {childIdentity, message.Position, message.Payload, message.Id, childIdentity.gameObject});
    }
}
