using UnityEngine;
using System.Collections;

public class AnimatorTestScript : Photon.PunBehaviour
{

    private Animator m_combatAnimator;
    private int m_trigger = 0;
    private bool m_isCombatStarted = false;

    // Use this for initialization
    override protected void Awake()
    {
        DontDestroyOnLoad(this);
        m_combatAnimator = GetComponent<Animator>();
        if (PhotonNetwork.isMasterClient)
        {
            m_combatAnimator.SetBool("StartToThink", true);
        }
    }

    /*
    void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            m_trigger += 1;
            if (m_trigger > 100 && !m_isCombatStarted)
            {
                m_isCombatStarted = true;
                m_combatAnimator.SetBool("StartCombat", true);
            }
        }
    }
    */
    /*
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            bool trigger = m_combatAnimator.GetBool("StartCombat");
            stream.SendNext(trigger);
        }
        else
        {
            bool isStartCombatSet = (bool)stream.ReceiveNext();
            if (isStartCombatSet)
            {
                m_combatAnimator.SetTrigger("StartCombat");
            }
        }
    }
    */
}