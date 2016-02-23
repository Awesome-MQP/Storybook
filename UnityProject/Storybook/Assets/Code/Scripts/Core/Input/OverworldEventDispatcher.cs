using UnityEngine;
using System.Collections;

public class OverworldEventDispatcher : EventDispatcher {

    public interface IOverworldEventListener : IEventListener
    {
        void SubmitDirection(Door.Direction directionToSubmit);
    }

    public void SubmitDirection(Door.Direction directionToSubmit)
    {
        foreach (IOverworldEventListener listener in IterateListeners<IOverworldEventListener>())
        {
            listener.SubmitDirection(directionToSubmit);
        }
    }
}
