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

    public void PagesGenerated(PageData[] pagesGenerated)
    {
        foreach (IShopEventListener listener in IterateListeners<IShopEventListener>())
        {
            listener.PagesGenerated(pagesGenerated);
        }
    }

    public void PageTraded(PageData pageTraded)
    {
        foreach (IShopEventListener listener in IterateListeners<IShopEventListener>())
        {
            listener.PageTraded(pageTraded);
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

    public void CombatSummarySubmitted()
    {
        foreach(ICombatSummaryListener listener in IterateListeners<ICombatSummaryListener>())
        {
            listener.CombatSummarySubmitted();
        }
    }

    public interface IShopEventListener : IEventListener
    {
        void OnShopClosed();

        void PagesGenerated(PageData[] pagesGenerated);

        void PageTraded(PageData pageTraded);
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

    public interface ICombatSummaryListener : IEventListener
    {
        void CombatSummarySubmitted();
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
