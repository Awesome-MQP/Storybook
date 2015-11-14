using UnityEngine;
using System.Collections;

public abstract class AssetPackageHandler
{
    public abstract void OnStartup();

    public abstract void OnLoaded();

    public abstract void OnShutdown();

    public abstract int GetPackageCount();

    public abstract AssetPackage GetPackage();
}
