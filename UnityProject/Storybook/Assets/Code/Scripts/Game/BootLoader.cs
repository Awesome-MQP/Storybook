
using UnityEngine;

class BootLoader
{
    [PostLoadMethod(Order = 100)]
    private static void _postLoad()
    {
        DefaultsResource defaults = Resources.Load<DefaultsResource>("Defaults");
    }
}
