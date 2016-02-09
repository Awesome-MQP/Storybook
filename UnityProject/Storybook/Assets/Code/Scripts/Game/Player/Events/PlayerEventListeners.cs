
public interface IPlayerRoomEventListener: IEventListener
{
    void OnEnteredRoom(BasePlayerMover mover, RoomObject room);
    void OnExitRoom(BasePlayerMover mover, RoomObject room);
    void OnPreRoomEvent(BasePlayerMover mover, RoomObject room);
    void OnPostRoomEvent(BasePlayerMover mover, RoomObject room);
    void OnWaitingForInput(BasePlayerMover mover);
}