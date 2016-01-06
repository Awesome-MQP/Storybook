
public class PlayerControlEventDispatcher : EventDispatcher
{
    public void OnWaitingForDirectionInput(PlayerControlEventListener.DirectionInputSelectedCallback callback)
    {
        foreach (PlayerControlEventListener listener in IterateListeners<PlayerControlEventListener>())
        {
            listener.OnWaitingForDirectionInput(callback);
        }
    }
}

public abstract class PlayerControlEventListener : IEventListener
{
    public EventDispatcher Dispatcher
    {
        get { return EventDispatcher.GetDispatcher<PlayerControlEventDispatcher>(); }
    }

    public delegate void DirectionInputSelectedCallback(Door.Direction direction);

    public abstract void OnWaitingForDirectionInput(DirectionInputSelectedCallback callback);
}