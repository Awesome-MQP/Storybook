using UnityEngine;
using System.Collections;

public class ShopUIEventDispatcher : EventDispatcher {

    public interface IShopEventListener : IEventListener
    {
        void OnShopClosed();

        void PagesGenerated(PageData[] pagesGenerated);

        void PageTraded(PageData pageTraded);
    }

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
}
