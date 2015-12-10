using UnityEngine;
using System.Collections;

public interface IEventListener
{
    EventDispatcher Dispatcher { get; }
}

public abstract class MonoEventListener : MonoBehaviour, IEventListener
{
    public abstract EventDispatcher Dispatcher { get; }

    protected virtual void Awake()
    {
        Dispatcher.RegisterEventListener(this);
    }
}
