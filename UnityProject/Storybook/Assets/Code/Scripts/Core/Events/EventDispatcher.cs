using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

/// <summary>
/// An object for dispatching events to listeners.
/// </summary>
public abstract class EventDispatcher
{
    //A table of all dispatchers sorted by there type.
    private static Dictionary<Type, EventDispatcher> s_dispatchers = new Dictionary<Type, EventDispatcher>();

    private LinkedList<IEventListener> m_listeners;
    private HashSet<IEventListener> m_listenerSet; 

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetDispatcher<T>() where T : EventDispatcher, new()
    {
        Type tType = typeof (T);
        EventDispatcher dispatcher;

        s_dispatchers.TryGetValue(tType, out dispatcher);
        T castedDispatcher = dispatcher as T;

        //This condition should never happen, if it does our table has been corrupted
        Assert.IsFalse(castedDispatcher == null && dispatcher != null);

        if (castedDispatcher == null)
        {
            castedDispatcher = new T();
            s_dispatchers.Add(tType, castedDispatcher);
        }

        return castedDispatcher;
    }

    protected EventDispatcher()
    {
        m_listeners = new LinkedList<IEventListener>();
        m_listenerSet = new HashSet<IEventListener>();
    }

    /// <summary>
    /// Registers an event listener with this dispatcher.
    /// </summary>
    /// <param name="listener">The listener to register.</param>
    public void RegisterEventListener(IEventListener listener)
    {
        if (m_listenerSet.Contains(listener))
            return;

        m_listeners.AddLast(listener);
        m_listenerSet.Add(listener);
    }

    /// <summary>
    /// Iterates through the registered listeners for this dispatcher.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected IEnumerable<T> IterateListeners<T>()
    {
        foreach (IEventListener eventListener in m_listeners)
        {
            if (eventListener is T)
            {
                yield return (T) eventListener;
            }
        }
    } 
}
