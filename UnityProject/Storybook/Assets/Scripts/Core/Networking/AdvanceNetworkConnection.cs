using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Advance connection to use with child spawning
/// </summary>
public class AdvanceNetworkConnection : NetworkConnection
{
    public override bool Send(short msgType, MessageBase msg)
    {
        Type messageType = msg.GetType();
        //Because of the problem with child spawning we intercept normal spawns and replace it with our own if we are under the correct conditions
        //This is highly illegal never do this
        if (messageType.Name == "ObjectSpawnMessage")
        {
            NetworkInstanceId networkId = (NetworkInstanceId) messageType.GetField("netId").GetValue(msg);

            GameObject foundObject = NetworkServer.FindLocalObject(networkId);
            if (foundObject == null)
                return base.Send(msgType, msg);

            ChildIdentity childId = foundObject.GetComponent<ChildIdentity>();
            if (childId == null)
                return base.Send(msgType, msg);

            byte[] payload = (byte[]) messageType.GetField("payload").GetValue(msg);

            //Intercept the message with a child message
            return base.Send(ChildSpawnMessage.MessageId, new ChildSpawnMessage(childId, payload));
        }

        return base.Send(msgType, msg);
    }

    public override bool SendByChannel(short msgType, MessageBase msg, int channelId)
    {
        Type messageType = msg.GetType();
        //Because of the problem with child spawning we intercept normal spawns and replace it with our own if we are under the correct conditions
        //This is highly illegal never do this
        if (messageType.Name == "ObjectSpawnMessage")
        {
            NetworkInstanceId networkId = (NetworkInstanceId)messageType.GetField("netId").GetValue(msg);

            GameObject foundObject = NetworkServer.FindLocalObject(networkId);
            if (foundObject == null)
                return base.SendByChannel(msgType, msg, channelId);

            ChildIdentity childId = foundObject.GetComponent<ChildIdentity>();
            if (childId == null)
                return base.SendByChannel(msgType, msg, channelId);

            byte[] payload = (byte[])messageType.GetField("payload").GetValue(msg);

            return base.SendByChannel(ChildSpawnMessage.MessageId, new ChildSpawnMessage(childId, payload), channelId);
        }

        return base.SendByChannel(msgType, msg, channelId);
    }
}
