using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TestCombatPawn : CombatPlayer, ICombatEventListener
{
    private int m_selectedHandIndex = -1;
    private bool m_isThinking = false;

    // Receive a move to use from the UI.
    public void OnCombatMoveChosen(int handNumber)
    {
        // Only do this if thinking!
        if(m_isThinking)
        {
            Debug.Log("Got a CombatMoveChosen event, index: " + handNumber);
            m_selectedHandIndex = handNumber;
        }
    }

    public void OnReceivePage(Page playerPage, int counter)
    {
        return;
    }

    public void OnPawnTakesDamage(PhotonPlayer thePlayer, int damageTaken)
    {
        return;
    }
    
    protected override void Awake()
    {
        base.Awake();
        EventDispatcher.GetDispatcher<CombatEventDispatcher>().RegisterEventListener(this);
    }

    // Waits for input of a move
    public override void OnThink()
    {
        //int numPressed = _getButtonPress();
        m_isThinking = true;
        // If the player hits the space bar and this pawn is the client's, submit a move
        if (m_selectedHandIndex > -1 && PhotonNetwork.player.ID == PawnId)
        {
            SelectedPageIndex = m_selectedHandIndex;

            Debug.Log("Submitted page " + m_selectedHandIndex);

            PhotonView m_scenePhotonView = GetComponent<PhotonView>();
            
            Page chosenPage = PlayerHand[m_selectedHandIndex];
            PlayerMove chosenMove = chosenPage.PlayerCombatMove;

            List<CombatPawn> targetList = new List<CombatPawn>();
            if (chosenPage.PageType == MoveType.Attack)
            {
                targetList.Add(GetPawnsOpposing()[0]);
            }
            else if (chosenPage.PageType == MoveType.Boost)
            {
                targetList.Add(GetPawnsOnTeam()[0]);
            }

            RemovePageFromHand(chosenPage);
            
            chosenMove.SetMoveOwner(this);
            chosenMove.SetMoveTargets(targetList);
            chosenMove.InitializeMove(); 
            SetMoveForTurn(chosenMove);
            SetHasSubmittedMove(true);
            
            PageMove pm = new PageMoveObject() as PageMove; // I had to create a child class that inherits PageMove in order to do stuff with it.
            pm.construct(chosenPage);

            int[] targetIds = new int[4];
            for (int i = 0; i < pm.NumberOfTargets; i++)
            {
                CombatPawn target = chosenMove.MoveTargets[i];
                targetIds[i] = target.PawnId;
            }

            GetComponent<PhotonView>().RPC("SendPlayerMoveOverNetwork", PhotonTargets.Others, PawnId, targetIds, m_selectedHandIndex);
            m_selectedHandIndex = -1; // Reset it.
            m_isThinking = false;
        }
    }

    private int _getButtonPress()
    {
        int numPressed = -1;
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            numPressed = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            numPressed = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            numPressed = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            numPressed = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            numPressed = 4;
        }
        return numPressed;
    }
}