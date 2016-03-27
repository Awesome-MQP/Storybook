using UnityEngine;
using System.Collections;

public class TutorialEventDispatcher : EventDispatcher {

    public void OnTutorialStart()
    {
        foreach (ITutorialEventListener listener in IterateListeners<ITutorialEventListener>())
        {
            listener.OnTutorialStart();
        }
    }

    public void OnPageForRoomUIOpened()
    {
        foreach (ITutorialEventListener listener in IterateListeners<ITutorialEventListener>())
        {
            listener.OnPageForRoomUIOpened();
        }
    }

    public void OnCombatStarted()
    {
        foreach (ITutorialEventListener listener in IterateListeners<ITutorialEventListener>())
        {
            listener.OnCombatStarted();
        }
    }

    public void OnCombatCleared()
    {
        foreach (ITutorialEventListener listener in IterateListeners<ITutorialEventListener>())
        {
            listener.OnCombatCleared();
        }
    }

    public void OnDeckManagementOpened()
    {
        foreach (ITutorialEventListener listener in IterateListeners<ITutorialEventListener>())
        {
            listener.OnDeckManagementOpened();
        }
    }

    public void OnShopEntered()
    {
        foreach (ITutorialEventListener listener in IterateListeners<ITutorialEventListener>())
        {
            listener.OnShopEntered();
        }
    }

    public void OnBossFightStarted()
    {
        foreach (ITutorialEventListener listener in IterateListeners<ITutorialEventListener>())
        {
            listener.OnBossFightStarted();
        }
    }

    public void OnDemoCompleted()
    {
        foreach (ITutorialEventListener listener in IterateListeners<ITutorialEventListener>())
        {
            listener.OnDemoCompleted();
        }
    }

    public void OnCharacterSelect()
    {
        foreach (ITutorialEventListener listener in IterateListeners<ITutorialEventListener>())
        {
            listener.OnCharacterSelect();
        }
    }

    public interface ITutorialEventListener: IEventListener 
    {
        void OnTutorialStart();
        void OnPageForRoomUIOpened();
        void OnCombatStarted();
        void OnCombatCleared();
        void OnDeckManagementOpened();
        void OnShopEntered();
        void OnBossFightStarted();
        void OnDemoCompleted();
        void OnCharacterSelect();
    }
}
