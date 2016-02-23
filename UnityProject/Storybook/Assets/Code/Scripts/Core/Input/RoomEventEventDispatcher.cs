using UnityEngine;
using System.Collections;

public class RoomEventEventDispatcher : EventDispatcher {

    public interface IRoomEventListener : IEventListener
    {
        void OnRoomCleared();
    }

    public void OnRoomCleared()
    {
        foreach (IRoomEventListener listener in IterateListeners<IRoomEventListener>())
        {
            listener.OnRoomCleared();
        }
    }

}
