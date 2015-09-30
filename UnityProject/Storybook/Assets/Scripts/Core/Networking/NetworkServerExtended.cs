using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkServerExtended
{
    public static void SpawnWithChildren(GameObject obj)
    {
        NetworkServer.Spawn(obj);

        ChildIdentity[] children = obj.GetComponentsInChildren<ChildIdentity>();
        foreach (ChildIdentity child in children)
        {
            NetworkServer.Spawn(child.gameObject);
        }
    }
}
