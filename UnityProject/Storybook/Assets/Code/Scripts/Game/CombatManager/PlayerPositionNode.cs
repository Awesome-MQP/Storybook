using UnityEngine;
using System.Collections;

public class PlayerPositionNode : MonoBehaviour {

    [SerializeField]
    private int m_positionId;

    public int PositionId
    {
        get { return m_positionId; }
    }
}
