using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [SerializeField] public List<CreatureData> allCreatures;
    [SerializeField] public List<EnemyData> allEnemies;
    [SerializeField] public List<Vector3> allPoop;
    [SerializeField] public List<ProjectileData> allProjectiles;
    [SerializeField] public List<FoodData> allFood;


    public GameData()
    {

        allCreatures = new List<CreatureData>();
        allEnemies = new List<EnemyData>();
        allPoop = new List<Vector3>();
        allProjectiles = new List<ProjectileData>();
        allFood = new List<FoodData>();
    }
}
