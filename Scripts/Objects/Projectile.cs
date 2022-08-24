using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private const int PROJECTILE_SPEED = 3;

    private int powerOfShooter;
    private GameManager.Faction factionOfShooter;
    private Vector3 direction;
    private Vector3 originalSpawnPosition;
    private float attackRange;


    public void InitValues(int powerOfShooter, GameManager.Faction factionOfShooter, Vector3 direction, float attackRange)
    {
        this.powerOfShooter = powerOfShooter;
        this.factionOfShooter = factionOfShooter;
        this.direction = Vector3.Normalize(direction);
        this.attackRange = attackRange;
        originalSpawnPosition = this.gameObject.transform.position;

        GetComponent<AudioSource>().volume = AudioManager.soundVolume;
        GetComponent<AudioSource>().Play();
    }

    void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(PROJECTILE_SPEED * Time.deltaTime * direction);

        if (Vector3.Distance(originalSpawnPosition, transform.position) > attackRange)
        {
            Destroy(gameObject);
        }

    }


    public void OnTriggerEnter(Collider other)
    {
        BattleUnit b = other.GetComponent<BattleUnit>();

        //if the projectile collides with a non BattleUnit, ignore it
        if (b == null)
        {
            return;
        }

        //no friendly fire, ignores already dead targets
        if (b.faction == this.factionOfShooter
            || b.currentBehaviour == BattleUnit.Behaviour.DEAD
            || b.currentBehaviour == BattleUnit.Behaviour.DECEASING)
        {
            return;
        }

        b.ReceiveDamage(powerOfShooter);
        Destroy(gameObject);


    }

    public ProjectileData CreateSaveData()
    {
        ProjectileData data = new ProjectileData();
        data.attackRange = attackRange;
        data.originalSpawnPosition = originalSpawnPosition;
        data.direction = direction;
        data.powerOfShooter = powerOfShooter;
        data.factionOfShooter = factionOfShooter;
        data.currentPosition = this.transform.position;
        return data;

    }

}
