using UnityEngine;
using System.Collections;

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

}
