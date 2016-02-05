using UnityEngine;
using System.Collections;
using System;

public class CombatEventDispatcher : EventDispatcher
{

    public void OnCheckingPawnStatus(CombatEventListener.PawnHasDiedCallback callback)
    {
        foreach (CombatEventListener listener in IterateListeners<CombatEventListener>())
        {
            listener.OnCheckingPawnStatus(callback);
        }
    }

    public void OnCheckingPawnHit(CombatEventListener.PawnIsHitCallback callback)
    {
        foreach (CombatEventListener listener in IterateListeners<CombatEventListener>())
        {
            listener.OnCheckingPawnHit(callback);
        }
    }

    //public void OnSendingPlayerHand(CombatEventListener.PawnSendingHandCallback callback, Page[] playerHand)
    public void OnSendingPageInfo(Page playerPage, int counter)
    {
        foreach (ICombatEventListener listener in IterateListeners<ICombatEventListener>())
        {
            listener.OnReceivePage(playerPage, counter);
        }
    }

    //public void OnPawnTakesDamage(CombatEventListener.PawnTakesDamageCallback callback, PhotonPlayer thePlayer, int damageTaken)
    public void OnPawnTakesDamage(PhotonPlayer thePlayer, int currentHealth, int maxHealth)
    {
        int i = 0;
        foreach (ICombatEventListener listener in IterateListeners<ICombatEventListener>())
        {
            listener.OnPawnTakesDamage(thePlayer, currentHealth, maxHealth);
            Debug.Log("Pawn taking damage " + i);
            i++;
        }
    }

    public void OnCombatMoveChosen(int pawnId, int handIndex, int[] targets)
    {
        foreach (ICombatEventListener listener in IterateListeners<ICombatEventListener>())
        {
            listener.OnCombatMoveChosen(pawnId, handIndex, targets);
        }
    }
}

// This interface handles any Combat related events between players and the UI.
// Primarily, it is meant to be used for players to pass their hands to the UI, and
// for the UI to know when to update a player's hitpoints.
public interface ICombatEventListener : IEventListener
{
    // Notifies the UI about receiving the player's hand
    void OnReceivePage(Page playerPage, int counter);

    // Notifies the UI about a player receiving damage
    void OnPawnTakesDamage(PhotonPlayer thePlayer, int currentHealth, int maxHealth);

    // Notifies a player about a page selection
    void OnCombatMoveChosen(int pawnId, int handIndex, int[] targets);
}

public abstract class CombatEventListener : IEventListener
{
    public EventDispatcher Dispatcher
    {
        get { return EventDispatcher.GetDispatcher<CombatEventDispatcher>(); }
    }

    protected CombatEventListener()
    {
        Dispatcher.RegisterEventListener(this);
    }

    public delegate void PawnHasDiedCallback(CombatPawn pawnThatDied);

    public abstract void OnCheckingPawnStatus(PawnHasDiedCallback callback);

    public delegate void PawnIsHitCallback(CombatPawn pawnHit, CombatMove moveUsed);

    public abstract void OnCheckingPawnHit(PawnIsHitCallback callback);

    public delegate void PawnSendingHandCallback(Page[] hand);

    public abstract void OnCheckingPlayerHand(PawnSendingHandCallback callback);

    public delegate void PawnTakesDamageCallback(PhotonPlayer thePlayer, int damageTaken);

    public abstract void OnPawnTakesDamage(PawnTakesDamageCallback callback);

}

/// <summary>
///  Class for handling UI-specific CombatEvents
///  The only methods in here we need to worry about are the OnCheckingPlayerHand and OnPawnTakesDamage
/// </summary>
public class CombatUIEventListener : CombatEventListener
{
    // We don't need this one to do anything.
    public override void OnCheckingPawnHit(PawnIsHitCallback callback)
    {
        return;
    }

    // We don't need this one to do anything.
    public override void OnCheckingPawnStatus(PawnHasDiedCallback callback)
    {
        return;
    }

    // This event is sent to the UI whenever a player draws a new hand
    public override void OnCheckingPlayerHand(PawnSendingHandCallback callback)
    {
        throw new NotImplementedException();
    }

    public override void OnPawnTakesDamage(PawnTakesDamageCallback callback)
    {
        throw new NotImplementedException();
    }
}