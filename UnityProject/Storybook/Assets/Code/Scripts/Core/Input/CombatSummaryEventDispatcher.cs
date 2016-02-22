using UnityEngine;
using System.Collections;

public class CombatSummaryEventDispatcher : EventDispatcher {

    public interface ICombatSummaryListener : IEventListener
    {
        void CombatSummarySubmitted();
    }

    public void CombatSummarySubmitted()
    {
        foreach (ICombatSummaryListener listener in IterateListeners<ICombatSummaryListener>())
        {
            listener.CombatSummarySubmitted();
        }
    }

}
