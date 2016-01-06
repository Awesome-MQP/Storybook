using UnityEngine;

public static class ComponentExtensions
{
    public static T GetComponentInParent<T>(this Component component, bool includeInactive) where T : Component
    {
        if (!includeInactive)
            return component.GetComponentInParent<T>();

        Transform t = component.transform;
        while (t)
        {
            T foundResult = t.GetComponent<T>();
            if (foundResult)
                return foundResult;

            t = t.parent;
        }

        return null;
    }
}
