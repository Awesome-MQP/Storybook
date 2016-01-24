using UnityEngine;
using System.Collections;

public interface IEventListener
{
    EventDispatcher Dispatcher { get; }
}

public abstract class EventListener<T> : IEventListener where T : EventDispatcher, new()
{
    public EventDispatcher Dispatcher
    {
        get { return EventDispatcher.GetDispatcher<T>(); }
    }

    protected EventListener()
    {
        Dispatcher.RegisterEventListener(this);
    } 
}

public abstract class MonoEventListener<T> : MonoBehaviour, IEventListener where T : EventDispatcher, new()
{
    public EventDispatcher Dispatcher
    {
        get { return EventDispatcher.GetDispatcher<T>(); }
    }

    protected virtual void Awake()
    {
        Dispatcher.RegisterEventListener(this);
    }
}
