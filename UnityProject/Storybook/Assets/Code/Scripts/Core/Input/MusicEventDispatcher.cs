using UnityEngine;
using System.Collections;

public class MusicEventDispatcher : EventDispatcher {

    public interface IMusicEventListener : IEventListener
    {
        void OnStartRoomEntered();
        void OnRoomMusicChange(Genre roomGenre);
        void OnCombatStart();
        void OnCombatEnd(Genre roomGenre);
        void OnShopEntered();
    }

    public void OnStartRoomEntered()
    {
        foreach (IMusicEventListener listener in IterateListeners<IMusicEventListener>())
        {
            listener.OnStartRoomEntered();
        }
    }

    public void OnRoomMusicChange(Genre roomGenre)
    {
        foreach (IMusicEventListener listener in IterateListeners<IMusicEventListener>())
        {
            listener.OnRoomMusicChange(roomGenre);
        }
    }

    public void OnCombatStart()
    {
        foreach (IMusicEventListener listener in IterateListeners<IMusicEventListener>())
        {
            listener.OnCombatStart();
        }
    }

    public void OnCombatEnd(Genre roomGenre)
    {
        foreach (IMusicEventListener listener in IterateListeners<IMusicEventListener>())
        {
            listener.OnCombatEnd(roomGenre);
        }
    }

    public void OnShopEntered()
    {
        foreach (IMusicEventListener listener in IterateListeners<IMusicEventListener>())
        {
            listener.OnShopEntered();
        }
    }
}
