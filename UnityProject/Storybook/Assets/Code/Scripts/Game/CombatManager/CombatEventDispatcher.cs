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
}
