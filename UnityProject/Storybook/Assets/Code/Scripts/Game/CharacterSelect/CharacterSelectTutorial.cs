using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CharacterSelectTutorial : Photon.PunBehaviour, TutorialEventDispatcher.ITutorialEventListener
{

    [SerializeField]
    TutorialUIHandler m_tutorialUIPrefab;

    public EventDispatcher TutorialDispatcher { get { return EventDispatcher.GetDispatcher<TutorialEventDispatcher>(); } }

    protected override void Awake()
    {
        EventDispatcher.GetDispatcher<TutorialEventDispatcher>().RegisterEventListener(this);

        base.Awake();
    }

    void OnDestroy()
    {
        EventDispatcher.GetDispatcher<TutorialEventDispatcher>().RemoveListener(this);
    }

    /// <summary>
    /// Creates the character select tutorial menu
    /// </summary>
    [PunRPC]
    public void CreateCharacterSelectTutorial()
    {
        List<string> tutorialStrings = new List<string>();
        tutorialStrings.Add("Character select text 1");
        tutorialStrings.Add("Character select text 2");

        _instantiateTutorialUI("Character Select", tutorialStrings);
    }

    private void _instantiateTutorialUI(string tutorialTitle, List<string> tutorialStrings)
    {
        GameObject tutorialUI = (GameObject)Instantiate(m_tutorialUIPrefab.gameObject, Vector3.zero, Quaternion.identity);
        TutorialUIHandler tutorialUIHandler = tutorialUI.GetComponent<TutorialUIHandler>();

        tutorialUIHandler.PopulateMenu(tutorialTitle, tutorialStrings);
    }

    public void OnTutorialStart()
    {
    }

    public void OnPageForRoomUIOpened()
    {
    }

    public void OnCombatStarted()
    {
    }

    public void OnCombatCleared()
    {
    }

    public void OnDeckManagementOpened()
    {
    }

    public void OnShopEntered()
    {
    }

    public void OnBossFightStarted()
    {
    }

    public void OnDemoCompleted()
    {
    }

    public void OnCharacterSelect()
    {
        photonView.RPC(nameof(CreateCharacterSelectTutorial), PhotonTargets.All);
    }
}
