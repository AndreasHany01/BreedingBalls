using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CreatureData
{

    //not saved: id, currentBehaviour, waypoints, enemy target, breedable time

    public Vector3 position;
    public bool male;
    public bool breedable;
    public Color32 color;
    public string name;
    public float speed;
    public int maxHp;
    public int currentHp;
    public int power;
    public GameManager.Faction faction;
    public int hunger;
    public int age;
    public int lastTimeBred;

}
