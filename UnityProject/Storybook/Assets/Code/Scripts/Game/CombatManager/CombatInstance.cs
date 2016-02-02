using UnityEngine;
using System.Collections;

public abstract class CombatInstance : MonoBehaviour
{
    public abstract CombatTeam[] CreateTeams();
}
