using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorybookPlayerMover : BasePlayerMover {

    List<PlayerWorldPawn> m_playerWorldPawns = new List<PlayerWorldPawn>();

    [SerializeField]
    List<PlayerNode> m_playerPositions = new List<PlayerNode>();

    public PlayerNode[] PlayerPositions
    {
        get { return m_playerPositions.ToArray(); }
    }
}
