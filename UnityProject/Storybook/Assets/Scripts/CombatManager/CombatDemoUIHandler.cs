using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CombatDemoUIHandler : MonoBehaviour {

    private CombatPawn[] m_playerPawns;
    private CombatEnemy[] m_enemyPawns;
    private bool m_isCombatStarted = false;

    [SerializeField]
    private Text m_player1Health;

    [SerializeField]
    private Text m_player1Speed;

    [SerializeField]
    private Text m_player2Health;

    [SerializeField]
    private Text m_player3Health;

    [SerializeField]
    private Text m_player4Health;

    [SerializeField]
    private Text m_enemy1Health;

    [SerializeField]
    private Text m_enemy1Speed;

    [SerializeField]
    private Text m_enemy2Health;

    [SerializeField]
    private Text m_enemy3Health;

    [SerializeField]
    private Text m_enemy4Health;

    [SerializeField]
    private Button m_attackButton;

    [SerializeField]
    private Button m_supportButton;

    private int m_chosenPageIndex;
    private bool m_isMoveChosen = false;

    void Update()
    {
        if (m_isCombatStarted)
        {
            // Player health values
            m_player1Health.text = "P1 Health = " + m_playerPawns[0].Health;
            m_player1Speed.text = "P1 Speed = " + m_playerPawns[0].Speed;
            if (m_playerPawns.Length > 1)
            {
                m_player2Health.text = "P2 Health = " + m_playerPawns[1].Health;
            }
            if (m_playerPawns.Length > 2)
            {
                m_player3Health.text = "P3 Health = " + m_playerPawns[2].Health;
            }
            if (m_playerPawns.Length > 3)
            {
                m_player4Health.text = "P4 Health = " + m_playerPawns[3].Health;
            }

            // Enemy health values
            m_enemy1Health.text = "Enemy 1 Health = " + m_enemyPawns[0].Health;
            m_enemy1Speed.text = "Enemy 1 Speed = " + m_enemyPawns[0].Speed;
            if (m_enemyPawns.Length > 1)
            {
                m_enemy2Health.text = "Enemy 2 Health = " + m_enemyPawns[1].Health;
            }
            if (m_enemyPawns.Length > 2)
            {
                m_enemy3Health.text = "Enemy 3 Health = " + m_enemyPawns[2].Health;
            }
            if (m_enemyPawns.Length > 3)
            {
                m_enemy4Health.text = "Enemy 4 Health = " + m_enemyPawns[3].Health;
            }
        }
    }

    public void DisableButtons()
    {
        m_attackButton.enabled = false;
        m_supportButton.enabled = false;
    }

    public void EnableButtons()
    {
        m_attackButton.enabled = true;
        m_supportButton.enabled = true;
        m_isMoveChosen = false;
    }

    public void OnAttackPressed()
    {
        DisableButtons();
        Debug.Log("Attack pressed");
        m_chosenPageIndex = 0;
        m_isMoveChosen = true;
    }

    public void OnSupportPressed()
    {
        DisableButtons();
        m_chosenPageIndex = 1;
        m_isMoveChosen = true;
    }

    public void SetPlayerPawns(CombatPawn[] newPlayerPawns)
    {
        m_playerPawns = newPlayerPawns;
    }

    public void SetEnemyPawns(CombatEnemy[] newEnemyPawns)
    {
        m_enemyPawns = newEnemyPawns;
    }

    public void SetIsCombatStarted(bool isCombatStarted)
    {
        m_isCombatStarted = isCombatStarted;
    }

    public bool IsMoveChosen
    {
        get { return m_isMoveChosen; }
    }

    public int ChosenIndex
    {
        get { return m_chosenPageIndex; }
    }

}
