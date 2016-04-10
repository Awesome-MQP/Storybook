public class PlayerControlEventDispatcher : EventDispatcher
{
    public void OnEnteredRoom(BasePlayerMover mover, RoomObject room)
    {
        foreach (IPlayerRoomEventListener listener in IterateListeners<IPlayerRoomEventListener>())
        {
            listener.OnEnteredRoom(mover, room);
        }
    }

    public void OnExitRoom(BasePlayerMover mover, RoomObject room)
    {
        foreach (IPlayerRoomEventListener listener in IterateListeners<IPlayerRoomEventListener>())
        {
            listener.OnExitRoom(mover, room);
        }
    }

    public void OnPreRoomEvent(BasePlayerMover mover, RoomObject room)
    {
        foreach (IPlayerRoomEventListener listener in IterateListeners<IPlayerRoomEventListener>())
        {
            listener.OnPreRoomEvent(mover, room);
        }
    }

    public void OnPostRoomEvent(BasePlayerMover mover, RoomObject room)
    {
        foreach (IPlayerRoomEventListener listener in IterateListeners<IPlayerRoomEventListener>())
        {
            listener.OnPostRoomEvent(mover, room);
        }
    }

    public void OnWaitingForDirectionInput(BasePlayerMover mover)
    {
        foreach (IPlayerRoomEventListener listener in IterateListeners<IPlayerRoomEventListener>())
        {
            listener.OnWaitingForInput(mover);
        }
    }
}