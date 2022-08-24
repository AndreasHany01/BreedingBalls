using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : BattleUnit
{
    public Camera cam;
    public GameObject poopPrefab;
    public GameObject callForHelpPrefab;

    public AudioSource audiosource;
    public AudioClip[] idleSounds;
    public AudioClip helpSound;
    public AudioClip eatingSound;



    public int callForHelpCooldown = 5;
    public float lastCallForHelpTime;
    private bool receivedHelpCall = false;
    private Vector3 helpCallFromPosition;

    public bool male;
    public bool breedable = false;
    public int hunger = STARTING_HUNGER;
    public int age = 0;

    private float hpScale = 0.01f;



    public Creature matingPartner;
    private Food food;

    public int lastTimeBred = 0;
    public const int BREEDING_MINIMUM_AGE = 10;
    public const int TIME_TILL_BREEDABLE_AFTER_BREEDING = 20;
    public const int STARTING_HUNGER = 40;
    public const int LOOK_FOR_FOOD_WITH_HUNGER = 38;

    // Start is called before the first frame update
    void Start()
    {

        //note that this is also called after loading from a save
        nextValidAttackTime = Time.time;
        InvokeRepeating("MoveRandomly", 3f + Random.Range(0, 4), 3f + Random.Range(0, 6));
        InvokeRepeating("Poop", 11f + Random.Range(0, 4), 21f + Random.Range(0, 6));
        InvokeRepeating("IncreaseHunger", 10f, 10f);
        InvokeRepeating("IncreaseAge", 1f, 1f);
        lastCallForHelpTime = Time.time;
        audiosource = GetComponent<AudioSource>();

    }

    private void MoveRandomly()
    {
        if (currentBehaviour == Behaviour.IDLE)
        {
            SetMoveTo(new Vector3(this.gameObject.transform.position.x + Random.Range(-2, 3), GameManager.OBJECT_HEIGHT, this.gameObject.transform.position.x + Random.Range(-3, 4)),
                Behaviour.MOVING);
            //GameObject p = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);

            if (Random.Range(0, 2) == 0)
            {

                audiosource.volume = AudioManager.soundVolume;
                audiosource.clip = idleSounds[Random.Range(0, idleSounds.Length)];
                audiosource.Play();
            }

        }

    }

    private void Poop()
    {
        if (currentBehaviour == Behaviour.IDLE)
        {

            GameObject p = Instantiate(poopPrefab, new Vector3(this.transform.position.x, 0, this.transform.position.z), Quaternion.identity);

            p.transform.parent = gamemanager.poopContainer.transform;
        }

    }


    private void IncreaseHunger()
    {
        hunger++;

        if (hunger > 100)
        {
            StartDyingProcess();
            return;
        }

    }

    private void IncreaseAge()
    {
        age++;

        if (age == BREEDING_MINIMUM_AGE || age - lastTimeBred >= TIME_TILL_BREEDABLE_AFTER_BREEDING)
        {
            breedable = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (currentBehaviour == Behaviour.DECEASING)
        {
            Decease();
            return;
        }

        if (currentBehaviour == Behaviour.DEAD)
        {
            return;
        }


        if (currentBehaviour == Behaviour.MOVING || currentBehaviour == Behaviour.MOVE_TO_BREED
            || currentBehaviour == Behaviour.MOVE_TO_FEED)
        {
            MoveLogic();
        }


        if (currentBehaviour == Behaviour.MOVE_AND_ATTACK)
        {

            BattleLogic();

            //if we still got a valid target, continue fighting
            if (enemyTarget != null)
            {
                return;
            }

            //if we received no help call, relax
            if (!receivedHelpCall)
            {
                currentBehaviour = Behaviour.IDLE;
                return;
            }

            //if we have no valid enemy target but received a help call, rush to the help caller
            moveTo = helpCallFromPosition;

            MoveLogic();

            //if we managed to get there without finding any enemies, relax
            if (ArrivedAtDestination())
            {

                currentBehaviour = Behaviour.IDLE;
                receivedHelpCall = false;
            }
        }

        if (currentBehaviour == Behaviour.IDLE && hunger > LOOK_FOR_FOOD_WITH_HUNGER)
        {
            currentBehaviour = Behaviour.MOVE_TO_FEED;
        }

    }

    private void FindNewFood()
    {
        Food nearestFood = null;
        float minDistance = float.MaxValue;

        foreach (GameObject o in gamemanager.allFood)
        {
            if (Vector3.Distance(this.transform.position, o.transform.position) < minDistance
                //&& Vector3.Distance(this.transform.position, o.transform.position) < aggroRange
                && !o.GetComponent<Food>().beingConsumed)
            {
                nearestFood = o.GetComponent<Food>();
                minDistance = Vector3.Distance(this.transform.position, o.transform.position);
            }
        }

        food = nearestFood;

    }

    private void MoveLogic()
    {

        //take current position of the breeding partner
        if (currentBehaviour == Behaviour.MOVE_TO_BREED)
        {

            //in case the matingPartner died
            if (matingPartner == null || matingPartner.currentBehaviour == Behaviour.DEAD)
            {
                currentBehaviour = Behaviour.IDLE;
                return;
            }

            SetMoveTo(matingPartner.transform.position, Behaviour.MOVE_TO_BREED);

        }
        else if (currentBehaviour == Behaviour.MOVE_TO_FEED)
        {
            if (food == null || food.beingConsumed)
            {
                FindNewFood();

                if (food == null)
                {
                    //TODO: maybe just wander around aimlessly to "find" food
                    return;
                }
                else
                {
                    moveTo = new Vector3(food.transform.position.x, GameManager.OBJECT_HEIGHT, food.transform.position.z);
                }
            }


        }

        //move to moveTo coordinates
        transform.Translate(speed * Time.deltaTime * Vector3.Normalize(new Vector3(moveTo.x, GameManager.OBJECT_HEIGHT, moveTo.z)
               - new Vector3(this.transform.position.x, GameManager.OBJECT_HEIGHT, this.transform.position.z)));



        if (ArrivedAtDestination())
        {
            if (currentBehaviour == Behaviour.MOVING)
            {
                currentBehaviour = Behaviour.IDLE;
            }
            else if (currentBehaviour == Behaviour.MOVE_TO_BREED)
            {
                currentBehaviour = Behaviour.BREED;
                breedable = false;
                StartCoroutine(FinishBreeding());
            }
            else if (currentBehaviour == Behaviour.MOVE_TO_FEED)
            {

                if (food == null || food.beingConsumed)
                {
                    currentBehaviour = Behaviour.IDLE;
                    return;
                }

                currentBehaviour = Behaviour.EATING;
                food.beingConsumed = true;


                audiosource.volume = AudioManager.soundVolume;
                audiosource.clip = eatingSound;
                audiosource.Play();

                StartCoroutine(FinishEating());
            }

        }
    }

    private bool ArrivedAtDestination()
    {
        return (Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(moveTo.x, moveTo.z)) < 0.3f);
    }


    private void OnMouseDown()
    {
        gamemanager.GetComponent<GameManager>().IHaveBeenChosen(this);
        // Debug.Log("I have been chosen! " + id + ", " + Time.timeSinceLevelLoad);
    }

    private bool spamPrevetionOn = false;

    private void OnMouseOver()
    {
        if (!spamPrevetionOn && Input.GetMouseButton(1))
        {
            //Debug.Log("rightclick on me?");
            gamemanager.GetComponent<GameManager>().IHaveBeenTargeted(this);
            spamPrevetionOn = true;
            StartCoroutine(DeactivateSpamPrevention());
        }
    }

    IEnumerator DeactivateSpamPrevention()
    {
        yield return new WaitForSeconds(0.2f);

        spamPrevetionOn = false;
    }

    IEnumerator FinishBreeding()
    {
        yield return new WaitForSeconds(2f);
        gamemanager.BreedingSuccess(this, matingPartner);
        currentBehaviour = Behaviour.IDLE;
        lastTimeBred = age;
    }

    IEnumerator FinishEating()
    {
        yield return new WaitForSeconds(2f);

        //eating process got interrupted
        if (currentBehaviour != Behaviour.EATING)
        {
            food.beingConsumed = false;
            yield break;
        }

        currentBehaviour = Behaviour.IDLE;
        hunger -= food.nutritionValue;

        if (hunger < 0)
        {
            hunger = 0;
        }


        gamemanager.allFood.Remove(food.gameObject);
        Destroy(food.gameObject);
        food = null;

    }



    public void InitValues(string name, float speed, int power, int maxHp, bool male, GameManager gamemanager, Camera cam)
    {
        this.id = GetId();
        this.myName = name;

        this.speed = speed;
        this.power = power;
        this.MaxHp = maxHp;
        //this.currentHp = maxHp;
        this.male = male;
        this.gamemanager = gamemanager;
        this.cam = cam;
        this.uimanager = gamemanager.uimanager;

        faction = GameManager.Faction.Creature;


        float sizeScale = hpScale * maxHp;

        if (sizeScale > 3)
        {
            sizeScale = 3;
        }

        this.transform.localScale = new Vector3(sizeScale, sizeScale, sizeScale);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.localScale.y / 2, this.transform.position.z);


    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Creature>() == null)
        {
            return;
        }

        //if another creature touches the idling creature, move randomly
        if (this.currentBehaviour == Behaviour.IDLE)
        {

            do
            {

                SetMoveTo(new Vector3(this.transform.position.x + Random.Range(-3, 4), GameManager.OBJECT_HEIGHT, this.transform.position.z + Random.Range(-3, 4)),
                    Behaviour.MOVING);

            } while (Vector3.Distance(this.transform.position, this.moveTo) < 2 * this.transform.localScale.x);

            return;
        }



    }

    public void ReceivingCallForHelp(CallForHelp c)
    {
        if (currentBehaviour == Behaviour.DEAD || currentBehaviour == Behaviour.DECEASING)
        {
            return;
        }

        if (c.faction == this.faction && !receivedHelpCall)
        {
            currentBehaviour = Behaviour.MOVE_AND_ATTACK;
            receivedHelpCall = true;
            helpCallFromPosition = c.positionOfHelpCaller;
        }
    }

    public void CryForHelp()
    {

        if (Time.time - lastCallForHelpTime > callForHelpCooldown)
        {

            Instantiate(callForHelpPrefab, new Vector3(this.transform.position.x, 0, this.transform.position.z), Quaternion.identity);
            audiosource.volume = AudioManager.soundVolume;
            audiosource.clip = helpSound;
            audiosource.Play();
        }
    }

    public CreatureData GetData()
    {
        CreatureData d = new CreatureData();
        d.position = this.transform.position;
        d.male = male;
        d.breedable = breedable;
        d.color = GetComponent<Renderer>().material.color;
        d.name = myName;
        d.speed = speed;
        d.maxHp = MaxHp;
        d.currentHp = currentHp;
        d.power = power;
        d.faction = faction;
        d.hunger = hunger;
        d.age = age;
        d.lastTimeBred = lastTimeBred;
        return d;
    }
}
