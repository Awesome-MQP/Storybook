using UnityEngine;
using System.Collections;

public class PageForRoomEventDispatcher : EventDispatcher {

    public interface IPageForRoomEventListener : IEventListener
    {
        void SubmitPageForRoom(PageData dataToSubmit);
    }

    public void SubmitPageForRoom(PageData dataToSubmit)
    {
        foreach (IPageForRoomEventListener listener in IterateListeners<IPageForRoomEventListener>())
        {
            listener.SubmitPageForRoom(dataToSubmit);
        }
    }
}
