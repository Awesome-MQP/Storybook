using UnityEngine;
using System.Collections;

public abstract class AssetPackage
{
    public bool IsLoaded
    {
        get { return m_loader.isDone; }
    }

    public AssetBundle Bundle
    {
        get { return m_loader.assetBundle; }
    }

    protected AssetPackage(string url)
    {
        m_loader = new WWW(url);
    }

    public virtual bool IsValid()
    {
#if UNITY_EDITOR
        return true;
#else
        return true;
#endif
    }

    /// <summary>
    /// Called after all packages have loaded.
    /// </summary>
    public abstract void OnStartup();

    public virtual void Unload()
    {
       m_loader.assetBundle.Unload(false);
    }

    private WWW m_loader;
}
