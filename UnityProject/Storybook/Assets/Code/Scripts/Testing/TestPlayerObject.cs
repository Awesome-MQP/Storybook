using System;
using UnityEngine;
using System.Collections;

public class TestPlayerObject : PlayerObject
{
    private int m_counter = 0;

    [SyncProperty]
    public int Counter
    {
        get { return m_counter; }
        private set
        {
            m_counter = value;
            PropertyChanged();
        }
    }

    void Update()
    {
        Counter++;
    }
}
