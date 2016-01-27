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

    public void OnSendingPlayerHand(CombatEventListener.PawnSendingHandCallback callback)
    {
        foreach (CombatEventListener listener in IterateListeners<CombatEventListener>())
        {
            listener.OnCheckingPlayerHand(callback);
        }
    }

    public void OnPawnTakesDamage(CombatEventListener.PawnTakesDamageCallback callback)
    {
        foreach (CombatEventListener listener in IterateListeners<CombatEventListener>())
        {
            listener.OnPawnTakesDamage(callback);
        }
    }
}

// This interface handles any Combat related events between players and the UI.
// Primarily, it is meant to be used for players to pass their hands to the UI, and
// for the UI to know when to update a player's hitpoints.
public interface ICombatEventListener : IEventListener
{

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

    public delegate void PawnTakesDamageCallback(int damageTaken);

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