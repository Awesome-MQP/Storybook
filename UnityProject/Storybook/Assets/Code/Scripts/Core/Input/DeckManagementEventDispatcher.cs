using UnityEngine;
using System.Collections;

public class DeckManagementEventDispatcher : EventDispatcher {

    public interface IDeckManagementEventListener : IEventListener
    {
        void OnDeckManagementClosed();
    }

    public void OnDeckManagementClosed()
    {
        foreach (IDeckManagementEventListener listener in IterateListeners<IDeckManagementEventListener>())
        {
            listener.OnDeckManagementClosed();
        }
    }
}
