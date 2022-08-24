using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAssistant
{
    public const int CREATURE_START_HP = 100;
    public const float DEFAULT_UNIT_SPEED = 2f;
    public const int CREATURE_START_POWER = 10;


    /// <summary>
    /// Creates and returns a standard enemy with the standard combat values.
    /// </summary>
    /// <returns>Returns a standard enemy with the standard combat values.</returns>
    public static EnemyData GetStandardEnemy()
    {
        EnemyData e = new EnemyData();
        e.maxHp = 100;
        e.currentHp = 100;
        e.power = 10;
        e.speed = 2;
        return e;
    }

    /// <summary>
    /// Returns a creature name based on the gender.
    /// </summary>
    /// <param name="male">True if the name is for a male creature, false if it's for a female.</param>
    /// <returns>
    /// Returns a creature name based on the gender.</returns>
    public static string GetCreatureName(bool male)
    {
        if (male)
        {
            string[] names = { "Hans", "Manu", "Acker", "Robin", "Klaus", "Andy", "Tron", "Mike", "Tom", "Batman" };

            return names[Random.Range(0, names.Length)];


        }
        else
        {


            string[] names = { "Becka", "Olivia", "Emma", "Ahri", "Sophia", "Mia", "Evelynn", "Ella", "Luna", "Mila", "Leona",
            "Lucy", "Naomi", "Maya"};

            return names[Random.Range(0, names.Length)];
        }
    }

}
