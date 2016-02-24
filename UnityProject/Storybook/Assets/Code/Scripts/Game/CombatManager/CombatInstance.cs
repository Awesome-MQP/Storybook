using System;
using UnityEngine;
using System.Collections;

public abstract class CombatInstance
{
    public abstract CombatTeam[] CreateTeams();
    public abstract AudioClip GetCombatMusic();
}
