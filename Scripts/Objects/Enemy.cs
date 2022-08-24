using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BattleUnit
{


    public void InitValues(GameManager gm, EnemyData data)
    {
        this.gamemanager = gm;
        this.uimanager = gm.uimanager;
        
        //if these variables are being set in the Start() method, the uimanager has already tried to use them
        this.id = GetId();
        faction = GameManager.Faction.Enemy;
        MaxHp = data.maxHp;
        currentHp = data.currentHp;
        speed = data.speed;
        power = data.power;
        currentBehaviour = Behaviour.MOVE_AND_ATTACK;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (currentBehaviour == Behaviour.DECEASING)
        {
            Decease();
            return;
        }

        if (currentBehaviour == Behaviour.MOVE_AND_ATTACK)
        {
            BattleLogic();
        }
    }

    public EnemyData GetData()
    {
        EnemyData d = new EnemyData();
        d.maxHp = MaxHp;
        d.currentHp = currentHp;
        d.speed = speed;
        d.power = power;
        d.position = this.transform.position;
        return d;

    }

}
