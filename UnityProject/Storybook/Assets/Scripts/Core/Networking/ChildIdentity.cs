using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ChildIdentity : MonoBehaviour
{
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