using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    /// <summary>
    /// Counts upwards each time a BattleUnit is being created, ensuring the generation of a unique id.
    /// </summary>
    protected static int globalId = 0;

    public int id;
    public string myName;
    public float speed;

    private int maxHp = 0;
    public int MaxHp
    {
        get { return maxHp; }
        set { 

            if (currentHp == maxHp)
            {
                maxHp = value;
                currentHp = maxHp;
            } else
            {
                maxHp = value;
            }
            
         }
    }
    public int currentHp = 0;
    public int power;
    public GameManager.Faction faction;
    public int attackRange = 8;
    public int aggroRange = 12;
    public float attackSpeed = 1.0f;
    public float nextValidAttackTime;


    public GameObject projectilePrefab;
    public GameManager gamemanager;
    public UIManager uimanager;

    /// <summary>
    /// Current attack target. Used for AI behaviour.
    /// </summary>
    public GameObject enemyTarget;

    public enum Behaviour { IDLE, MOVING, MOVE_TO_BREED, MOVE_TO_FEED, BREED, EATING, MOVE_AND_ATTACK, DEAD, DECEASING };
    public Behaviour currentBehaviour = Behaviour.IDLE;

    protected Vector3 moveTo;

    /// <summary>Method <c>BattleLogic</c> looks for the next enemy target. If in range it orders an attack,
    /// if out of range it orders a movement towards the enemy target.</summary>
    protected void BattleLogic()
    {
        //if the enemy target is dead or deceasing, find a new target
        if (enemyTarget == null 
            || enemyTarget.GetComponent<BattleUnit>().currentBehaviour == Behaviour.DEAD
            || enemyTarget.GetComponent<BattleUnit>().currentBehaviour == Behaviour.DECEASING)
        {
            enemyTarget = SearchNextEnemyTarget();

            if (enemyTarget == null)
            {
                return;
            }
        }

        //if the target is in range
        if (IsCurrentTargetInRange()) {

            //if the shoot cooldown is over shoot
            if (Time.time >= nextValidAttackTime)
            {
                nextValidAttackTime = Time.time + attackSpeed;
                Shoot();
            }

        //if the target is out of range, move towards it
        } else {

            nextValidAttackTime = Time.time + attackSpeed;

            SetMoveTo(enemyTarget.transform.position, Behaviour.MOVE_AND_ATTACK );

            //move to moveTo coordinates
            transform.Translate(speed * Time.deltaTime 
                * Vector3.Normalize(new Vector3(moveTo.x, GameManager.OBJECT_HEIGHT, moveTo.z)
                   - new Vector3(this.transform.position.x, GameManager.OBJECT_HEIGHT, this.transform.position.z)));


        }

    }

    /// <summary>
    /// Creates and returns a unique id.
    /// </summary>
    /// <returns>Returns a unique id.</returns>
    protected static int GetId()
    {
        globalId++;
        return globalId;
    }

    /// <summary>
    /// Returns the closest valid enemy target.
    /// </summary>
    /// <returns>Returns the closest valid enemy target.</returns>
    protected GameObject SearchNextEnemyTarget()
    {
        LinkedList<GameObject> possibleTargets;

        if (faction == GameManager.Faction.Creature)
        {
            possibleTargets = gamemanager.allEnemies;
        } else
        {

            possibleTargets = gamemanager.allCreatures;
        }

        GameObject nearestTarget = null;
        float minDistance = float.MaxValue;


        foreach (GameObject o in possibleTargets)
        {
            if (Vector3.Distance(this.transform.position, o.transform.position) < minDistance
                && o.gameObject.GetComponent<BattleUnit>().currentBehaviour != Behaviour.DEAD
                && o.gameObject.GetComponent<BattleUnit>().currentBehaviour != Behaviour.DECEASING
                && Vector3.Distance(this.transform.position, o.transform.position) < aggroRange)
            {
                nearestTarget = o;
                minDistance = Vector3.Distance(this.transform.position, o.transform.position);
            }
        }

        return nearestTarget;

    }

    /// <summary>
    /// Checks if the current <code>enemyTarget</code> is in attack range. Considers the size of the projectile prefab.
    /// </summary>
    /// <returns>True if the <code>enemyTarget</code> is in attack range, otherwise false.</returns>
    protected bool IsCurrentTargetInRange()
    {
        return (Vector3.Distance(enemyTarget.transform.position, this.transform.position) <= attackRange - projectilePrefab.GetComponent<SphereCollider>().radius);
    }

    protected void Shoot()
    {
        GameObject p = Instantiate(projectilePrefab, new Vector3(this.transform.position.x, 0.5f, this.transform.position.z), Quaternion.identity);
        p.GetComponent<Projectile>().InitValues(power, faction, enemyTarget.transform.position - this.transform.position, attackRange);
        p.transform.parent = gamemanager.projectileContainer.transform;

    }


    protected void StartDyingProcess()
    {

        currentBehaviour = Behaviour.DEAD;
        Renderer r = gameObject.GetComponent<Renderer>();
        r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, 0.5f);

        if (faction == GameManager.Faction.Creature)
        {

            StartCoroutine(StartDeceasingAfter(GameManager.TIME_BEFORE_DECEASING_CREATURE));
        }
        else
        {

            StartCoroutine(StartDeceasingAfter(GameManager.TIME_BEFORE_DECEASING_ENEMY));
        }
    }

    public virtual void ReceiveDamage(int d)
    {

        if (currentBehaviour == Behaviour.DECEASING || currentBehaviour == Behaviour.DEAD)
        {
            return;
        }

        if (faction == GameManager.Faction.Enemy)
        {
           // Debug.Log("getting hit! HP drops from " + currentHp + " to " + (currentHp - d) + "!");
        }

        currentHp -= d;
        currentBehaviour = Behaviour.MOVE_AND_ATTACK;
        uimanager.DisplayDamageNumbers(this.gameObject, d);


        if (currentHp <= 0)
        {
            StartDyingProcess();
        } else
        {

            if (faction == GameManager.Faction.Creature)
            {
                ((Creature)(this)).CryForHelp();


            }


        }

    }


    IEnumerator StartDeceasingAfter(float x)
    {
        yield return new WaitForSeconds(x);
        gamemanager.RemoveUnit(this.gameObject);
        currentBehaviour = Behaviour.DECEASING;
    }

    protected void Decease()
    {
            transform.Translate(0.2f * Time.deltaTime * new Vector3(0, -1, 0));

            if (this.transform.position.y < -6)
            {

                Destroy(gameObject);
            }
        
    }

    public void SetMoveTo(Vector3 v, Behaviour behaviour)
    {
        moveTo = v;
        this.currentBehaviour = behaviour;


        //---
        //respect the terrarium boundaries
        if (moveTo.x < -gamemanager.terrariumWidth / 2)
        {
            moveTo = new Vector3(-gamemanager.terrariumWidth / 2, moveTo.y, moveTo.z);
        }

        //no else if, to prevent a Gemüsli to run over a corner
        if (moveTo.z < -gamemanager.terrariumLength / 2)
        {
            moveTo = new Vector3(moveTo.x, moveTo.y, -gamemanager.terrariumLength / 2);
        }

        if (moveTo.x > gamemanager.terrariumLength / 2)
        {
            moveTo = new Vector3(gamemanager.terrariumWidth / 2, moveTo.y, moveTo.z);
        }

        if (moveTo.z > gamemanager.terrariumLength / 2)
        {
            moveTo = new Vector3(moveTo.x, this.moveTo.y, gamemanager.terrariumLength / 2);
        }
        //---
    }

}
