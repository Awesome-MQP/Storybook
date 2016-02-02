using UnityEngine;
using System.Collections;

public class TargetButton : MonoBehaviour {

    [SerializeField]
    private int m_targetId;

    public void OnClick()
    {
        UIHandler currentUIHandler = FindObjectOfType<UIHandler>();
        if (currentUIHandler is CombatMenuUIHandler)
        {
            CombatMenuUIHandler combatUIHandler = (CombatMenuUIHandler)currentUIHandler;
            combatUIHandler.TargetSelected(m_targetId);
        }
    }

    public int TargetId
    {
        get { return m_targetId; }
        set { m_targetId = value; }
    }

}
