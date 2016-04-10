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
        tutorialStrings.Add("Welcome to Storybook!");
        tutorialStrings.Add("Before starting a game, every player must choose their character, and only one of each is allowed per game.");
        tutorialStrings.Add("Each character represents a different genre, which represents a type in this game.");
        tutorialStrings.Add("Each genre is strong against a particular genre and weak to another.");

        tutorialStrings.Add("The comic book character is a strong attacker, is good against horror characters, but is weak to sci-fi characters.");
        tutorialStrings.Add("The sci-fi character has heavy defense, is good against comic book characters, but is weak to fantasy characters.");
        tutorialStrings.Add("The fantasy character is speedy and great for support, is good against sci-fi characters, but is weak to horror characters.");
        tutorialStrings.Add("The horror character is a tank with higher HP, is good against fantasy characters, but is weak to comic book characters.");

        tutorialStrings.Add("You may now choose the character you wish to play as.");

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
