using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileData
{

    public int powerOfShooter;
    public GameManager.Faction factionOfShooter;
    public Vector3 direction;
    public Vector3 originalSpawnPosition;
    public float attackRange;

    /// <summary>
    /// The current position of the projectile. It'll fly into the <code>direction</code> until it has reached
    /// a travel distance equal or greater to the <code>attackRange</code> of the shooter. The travel distance
    /// is calculated by the <code>>originalSpawnPosition</code> and the current position.
    /// </summary>
    public Vector3 currentPosition;
}
