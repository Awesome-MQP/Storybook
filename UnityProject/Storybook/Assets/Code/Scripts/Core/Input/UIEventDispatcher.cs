using UnityEngine;
using System.Collections;

public class UIEventDispatcher : EventDispatcher {

    public void OnShopClosed()
    {
        foreach (IShopEventListener listener in IterateListeners<IShopEventListener>())
        {
            listener.OnShopClosed();
        }
    }

    public void SubmitPageForRoom(PageData dataToSubmit)
    {
        foreach (IPageForRoomEventListener listener in IterateListeners<IPageForRoomEventListener>())
        {
            listener.SubmitPageForRoom(dataToSubmit);
        }
    }

    public void OnDeckManagementClosed()
    {
        foreach(IDeckManagementEventListener listener in IterateListeners<IDeckManagementEventListener>())
        {
            listener.OnDeckManagementClosed();
        }
    }

    public void SubmitDirection(Door.Direction directionToSubmit)
    {
        foreach(IOverworldEventListener listener in IterateListeners<IOverworldEventListener>())
        {
            listener.SubmitDirection(directionToSubmit);
        }
    }

    public void OnRoomCleared()
    {
        foreach(IRoomEventListener listener in IterateListeners<IRoomEventListener>())
        {
            listener.OnRoomCleared();
        }
    }

    public interface IShopEventListener : IEventListener
    {
        void OnShopClosed();
    }

    public interface IPageForRoomEventListener : IEventListener
    {
        void SubmitPageForRoom(PageData dataToSubmit);
    }

    public interface IDeckManagementEventListener : IEventListener
    {
        void OnDeckManagementClosed();
    }

    public interface IOverworldEventListener : IEventListener
    {
        void SubmitDirection(Door.Direction directionToSubmit);
    }

    public interface IRoomEventListener : IEventListener
    {
        void OnRoomCleared();
    }

    public abstract class UIEventListener : IEventListener
    {
        public EventDispatcher Dispatcher
        {
            get { return EventDispatcher.GetDispatcher<CombatEventDispatcher>(); }
        }

        protected UIEventListener()
        {
            Dispatcher.RegisterEventListener(this);
        }
    }
}
