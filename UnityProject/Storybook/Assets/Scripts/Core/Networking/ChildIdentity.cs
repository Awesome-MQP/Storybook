using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Used to identify a child across the network, add to a prefab to have its children spawn on the network.
/// </summary>
[ExecuteInEditMode]
public class ChildIdentity : MonoBehaviour
{
    /// <summary>
    /// The id of the child
    /// </summary>
    public int ChildId
    {
        get { return m_childId; }
    }

#if UNITY_EDITOR
    void Start()
    {
        Assert.IsNotNull(transform.parent, "A child identity must be the child of some object.");

        if (Application.isEditor && !Application.isPlaying && m_childId == 0)
            m_childId = Random.Range(0, int.MaxValue);
    }
#endif

    [SerializeField]
    private int m_childId = 0;
}