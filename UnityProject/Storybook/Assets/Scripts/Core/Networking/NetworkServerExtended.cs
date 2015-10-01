using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkServerExtended
{
    /// <summary>
    /// Spawns a network object with children.
    /// </summary>
    /// <param name="obj">The object to spawn with children.</param>
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
