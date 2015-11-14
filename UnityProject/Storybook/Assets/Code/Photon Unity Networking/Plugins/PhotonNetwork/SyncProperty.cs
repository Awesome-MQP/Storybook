using System;
using UnityEngine;
using System.Collections;

public class SyncProperty : Attribute
{
    public bool IsReliable
    {
        get { return m_isReliable; }
        set { m_isReliable = value; }
    }

    public SyncProperty()
    {
        m_isReliable = true;
    }

    private bool m_isReliable;
}
