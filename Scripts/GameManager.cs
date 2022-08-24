using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject creaturePrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject poopPrefab;
    [SerializeField] public UIManager uimanager;
    [SerializeField] private GameObject playerMessenger;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject introPic;
    public GameObject poopContainer;
    public GameObject foodContainer;
    public GameObject projectileContainer;
    public GameObject creatureContainer;
    public Camera cam;

    /// <summary>
    /// All creatures that are controllable by the player.
    /// </summary>
    public LinkedList<GameObject> allCreatures;

    public LinkedList<GameObject> allEnemies;

    public LinkedList<GameObject> allFood;

    [HideInInspector] public float terrariumLength = 0;
    [HideInInspector] public float terrariumWidth = 0;

    [HideInInspector] public Creature selectedByPlayer;


    [HideInInspector] public bool ignoreClickingDefaultBehavourForThisTick = false;

    /// <summary>
    /// Used for spawning objects. Should the ground/plane change, we can change the spawn
    /// height of all objects with this constant.
    /// </summary>
    [HideInInspector] public const int OBJECT_HEIGHT = 0;

    /// <summary>
    /// How many seconds a dead creature lies around before it vanishes.
    /// </summary>
    [HideInInspector] public const float TIME_BEFORE_DECEASING_CREATURE = 20;

    /// <summary>
    /// How many seconds a dead enemy lies around before it vanishes.
    /// </summary>
    [HideInInspector] public const float TIME_BEFORE_DECEASING_ENEMY = 3;

    public enum Faction { Creature, Enemy };

    private bool startScreen = true;

    // Start is called before the first frame update
    void Start()
    {
        allCreatures = new LinkedList<GameObject>();
        allEnemies = new LinkedList<GameObject>();
        allFood = new LinkedList<GameObject>();
        uimanager = GetComponent<UIManager>();


        terrariumLength = ground.transform.localScale.z * 10;
        terrariumWidth = ground.transform.localScale.x * 10;
        //if width != length the tiling could get stretched funnily
        ground.GetComponent<Ground>().AdjustTiling(terrariumWidth / 10, terrariumLength / 10);

        cam.GetComponent<MyCameraScript>().Init(this);
        GetComponent<FoodManager>().Init(this);

    }

    void StartGame()
    {

        introPic.transform.parent.gameObject.SetActive(false);
        SpawnCreature(new Vector3(0, OBJECT_HEIGHT, 0), "Bob", SpawnAssistant.DEFAULT_UNIT_SPEED, SpawnAssistant.CREATURE_START_POWER,
            SpawnAssistant.CREATURE_START_HP, true, true, new Color32(255, 0, 0, 255));
        SpawnCreature(new Vector3(3, OBJECT_HEIGHT, 1), "Alice", SpawnAssistant.DEFAULT_UNIT_SPEED, SpawnAssistant.CREATURE_START_POWER,
            SpawnAssistant.CREATURE_START_HP, false, true, new Color32(0, 0, 255, 255));


        bool male = true;

        if (Random.Range(0, 2) == 0)
        {
            male = false;
        }

        SpawnCreature(new Vector3(-3, OBJECT_HEIGHT, -3), SpawnAssistant.GetCreatureName(male), SpawnAssistant.DEFAULT_UNIT_SPEED, SpawnAssistant.CREATURE_START_POWER,
            SpawnAssistant.CREATURE_START_HP, male, true, new Color32(0, 255, 0, 255));

        foreach (GameObject g in allCreatures)
        {
            g.GetComponent<Creature>().age = 30;
        }
    }

    private GameObject SpawnCreature(Vector3 position, string name, float speed, int power, int maxHp, bool male, bool breedable, Color32 color)
    {


        GameObject o = Instantiate(creaturePrefab, position, Quaternion.identity);
        o.GetComponent<Creature>().InitValues(name, speed, power, maxHp, male, this, cam);
        o.GetComponent<Creature>().breedable = breedable;
        uimanager.AddCreature(o);
        allCreatures.AddLast(o);
        o.gameObject.GetComponent<Renderer>().material.color = color;
        o.transform.SetParent(creatureContainer.transform);
        return o;


    }

    // Update is called once per frame
    void Update()
    {
    }

    private void LateUpdate()
    {
        //Basically we need this as the creature messages the game manager if they have
        //been clicked on, but noone tells the game manager if the user clicked on empty space.
        //So we always assume the user clicked on empty space, except if this flag here has
        //be activated
        if (ignoreClickingDefaultBehavourForThisTick)
        {
            ignoreClickingDefaultBehavourForThisTick = false;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            selectedByPlayer = null;
            GetComponent<UIManager>().DisplayStats(null);

            if (startScreen)
            {
                startScreen = false;
                StartGame();
            }
        }

        //this logic shall also work if the behaviour is "Move_To_Breed", so the player can cancel the breeding action
        if (Input.GetMouseButtonDown(1) && selectedByPlayer != null
            && selectedByPlayer.currentBehaviour != Creature.Behaviour.BREED
            && selectedByPlayer.currentBehaviour != Creature.Behaviour.DEAD
            && selectedByPlayer.currentBehaviour != Creature.Behaviour.DECEASING)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {

                selectedByPlayer.SetMoveTo(hit.point, Creature.Behaviour.MOVING);
            }

        }
    }

    /// <summary>
    /// This gets called if the player clicked on a creature. Its stats will get displayed.
    /// </summary>
    /// <param name="g">The creature who has been clicked on.</param>
    public void IHaveBeenChosen(Creature g)
    {
        selectedByPlayer = g;
        GetComponent<UIManager>().DisplayStats(g);

        ignoreClickingDefaultBehavourForThisTick = true;

    }

    public void BreedingSuccess(Creature g1, Creature g2)
    {
        //This function can only get called by the female version.
        //Otherwise both creatures would call this method and we would have to
        //store past pairings or this method gets executed twice.
        //Alternatively we could add code in the creature class to only call
        //this if they're female, but that sounds worse design-wise.
        if (g1.male)
        {
            return;
        }

        float mutationSpeed = 0;
        int mutationPower = 0;
        int mutationHp = 0;
        float mutationRed = 0;
        float mutationGreen = 0;
        float mutationBlue = 0;


        //chance for mutation
        if (Random.Range(1, 10) == 1)
        {
            Debug.Log("Mutation!");
            mutationSpeed = Random.Range(0, 0.2f);
            mutationPower = Random.Range(0, 3);
            mutationHp = Random.Range(0, 3);
            mutationRed = Random.Range(-0.3f, 0.3f);
            mutationGreen = Random.Range(-0.3f, 0.3f);
            mutationBlue = Random.Range(-0.3f, 0.3f);
        }


        int powerRandom = Random.Range(-1, 2);
        int hpRandom = Random.Range(-1, 2);
        int power = (int)((g1.power + g2.power) / 2) + powerRandom + mutationPower;
        int maxHp = (int)((g1.MaxHp + g2.MaxHp) / 2) + hpRandom + mutationHp;
        float speed = (g1.speed + g2.speed) / 2 + Random.Range(-0.2f, 0.2f) + mutationSpeed;
        speed = Mathf.Round(speed * 100f) / 100f;

        int randomGender = Random.Range(0, 2);



        float red = g1.gameObject.GetComponent<Renderer>().material.color.r;
        float green = g1.gameObject.GetComponent<Renderer>().material.color.g;
        float blue = g1.gameObject.GetComponent<Renderer>().material.color.b;
        float red2 = g2.gameObject.GetComponent<Renderer>().material.color.r;
        float green2 = g2.gameObject.GetComponent<Renderer>().material.color.g;
        float blue2 = g2.gameObject.GetComponent<Renderer>().material.color.b;

        Color32 color = new Color(
            (red + red2) / 2 + Random.Range(-0.1f, 0.1f) + mutationRed,
            (green + green2) / 2 + Random.Range(-0.1f, 0.1f) + mutationGreen,
            (blue + blue2) / 2 + Random.Range(-0.1f, 0.1f) + mutationBlue,
            1f);


        SpawnCreature(g1.transform.position, SpawnAssistant.GetCreatureName(randomGender == 0), speed, power, maxHp, randomGender == 0, false, color);

    }

    /// <summary>
    /// This method is being called by a creature if it has been right-clicked on, which should
    /// initialize the breeding process if a suited other creature has been selected first.
    /// </summary>
    /// <param name="g">The creature that has been right-clicked on.</param>
    public void IHaveBeenTargeted(Creature g)
    {

        if (selectedByPlayer == null)
        {
            Debug.Log("no primary creature selected");
            return;
        }

        //both creatures must be alive, they must be breedable and have different genders
        if (selectedByPlayer.breedable && g.breedable && selectedByPlayer != g && selectedByPlayer.male != g.male
            && selectedByPlayer.currentBehaviour != BattleUnit.Behaviour.DEAD && selectedByPlayer.currentBehaviour != BattleUnit.Behaviour.DECEASING
            && g.currentBehaviour != BattleUnit.Behaviour.DEAD && g.currentBehaviour != BattleUnit.Behaviour.DECEASING)
        {
            ignoreClickingDefaultBehavourForThisTick = true;
            g.matingPartner = selectedByPlayer;
            g.currentBehaviour = Creature.Behaviour.MOVE_TO_BREED;
            selectedByPlayer.matingPartner = g;
            selectedByPlayer.currentBehaviour = Creature.Behaviour.MOVE_TO_BREED;
        }
        else
        {
            if (!selectedByPlayer.breedable)
            {
                DisplayMessageToPlayer(selectedByPlayer.myName + " not fertile right now!");
            }
            else if (!g.breedable)
            {
                DisplayMessageToPlayer(g.myName + " not fertile right now!");
            }
            else if (selectedByPlayer.currentBehaviour == BattleUnit.Behaviour.DEAD || selectedByPlayer.currentBehaviour == BattleUnit.Behaviour.DECEASING
                || g.currentBehaviour == BattleUnit.Behaviour.DEAD || g.currentBehaviour == BattleUnit.Behaviour.DECEASING)
            {
                DisplayMessageToPlayer("Necrophilia is not a feature, sorry.");
            }
            else if (selectedByPlayer.male == g.male)
            {
                DisplayMessageToPlayer("These two have the same sex!");
            }
        }

    }


    public void DisplayMessageToPlayer(string s)
    {
        playerMessenger.GetComponent<MessageToPlayer>().DisplayMessageToPlayer(s);
    }

    public void SpawnEnemy()
    {
        Vector3 spawnPosi = new Vector3(0, OBJECT_HEIGHT + 0.5f, 0);
        bool validSpawnPosi = true;

        do
        {
            validSpawnPosi = true;
            spawnPosi = new Vector3(Random.Range(-10, 10), OBJECT_HEIGHT + 0.5f, Random.Range(-10, 10));

            foreach (GameObject g in allCreatures)
            {
                Vector3 gPosi = g.transform.position;
                if (Mathf.Round(gPosi.x) == spawnPosi.x && Mathf.Round(gPosi.z) == spawnPosi.z)
                {
                    validSpawnPosi = false;
                    break;
                } 
            }

        } while (!validSpawnPosi);

        GameObject o = Instantiate(enemyPrefab, spawnPosi, Quaternion.identity);
        EnemyData d = SpawnAssistant.GetStandardEnemy();
        AfterEnemySpawn(o, d);
    }


    private void AfterEnemySpawn(GameObject enemy, EnemyData data)
    {
        enemy.GetComponent<Enemy>().InitValues(this, data);
        allEnemies.AddLast(enemy);
        uimanager.AddEnemy(enemy);

        GetComponent<AudioManager>().PlayMusic(AudioManager.Music.Battle);
    }

    /// <summary>
    /// Removes a gameobject with a <code>BattleUnit</code> script out of all lists. 
    /// Also changes the music to peaceful if the last enemy has been removed this way.
    /// The gameobject isn't being destroyed, as it'll lie around with the <code>currentBehavior</code>
    /// set to <code>Behaviour.DECEASING</code> first and it'll destroy itself later.
    /// </summary>
    /// <param name="ob">The gameobject which shall get removed from all lists.</param>
    public void RemoveUnit(GameObject ob)
    {

        uimanager.RemoveUnit(ob.GetComponent<BattleUnit>().id);

        if (ob.GetComponent<Creature>() != null)
        {
            allCreatures.Remove(ob);
        }
        else if (ob.GetComponent<Enemy>() != null)
        {
            allEnemies.Remove(ob);

            if (allEnemies.Count == 0)
            {
                GetComponent<AudioManager>().PlayMusic(AudioManager.Music.Peaceful);
            }
        }

    }
    public void Save(string saveName)
    {
        DataPersistenceManager.instance.SaveGame(saveName);


    }
    public void Load(string saveName)
    {

        DataPersistenceManager.instance.LoadGame(saveName);


    }


    public void LoadData(GameData data)
    {
        //remove all creatures from the scene
        foreach (GameObject g in allCreatures)
        {
            uimanager.RemoveUnit(g.GetComponent<BattleUnit>().id);
            Destroy(g);
        }

        allCreatures = new LinkedList<GameObject>();

        //spawn new creatures
        foreach (CreatureData d in data.allCreatures)
        {

            GameObject o = SpawnCreature(d.position, d.name, d.speed, d.power, d.maxHp, d.male, d.breedable, d.color);
            Creature creature = o.GetComponent<Creature>();
            creature.currentHp = d.currentHp;
            creature.hunger = d.hunger;
            creature.age = d.age;
            creature.lastTimeBred = d.lastTimeBred;
        }


        foreach (Transform t in poopContainer.transform)
        {
            Destroy(t.gameObject);
        }


        foreach (Vector3 v in data.allPoop)
        {

            GameObject p = Instantiate(poopPrefab, v, Quaternion.identity);
            p.transform.parent = poopContainer.transform;
            p.GetComponent<Poop>().loadedFromSave = true;

        }

        foreach (FoodData f in data.allFood)
        {
            GetComponent<FoodManager>().SpawnFood(f);

        }



        //remove all enemies from the scene
        foreach (GameObject g in allEnemies)
        {
            uimanager.RemoveUnit(g.GetComponent<BattleUnit>().id);
            Destroy(g);
        }

        allEnemies = new LinkedList<GameObject>();

        //spawn new enemies
        foreach (EnemyData d in data.allEnemies)
        {
            GameObject o = Instantiate(enemyPrefab, d.position, Quaternion.identity);
            AfterEnemySpawn(o, d);
            o.GetComponent<Enemy>().currentHp = d.currentHp;
        }



        foreach (Projectile p in FindObjectsOfType(typeof(Projectile)) as Projectile[])
        {
            Destroy(p.gameObject);
        }

        //spawn new projectiles
        foreach (ProjectileData d in data.allProjectiles)
        {

            GameObject p = Instantiate(projectilePrefab, d.currentPosition, Quaternion.identity);
            p.GetComponent<Projectile>().InitValues(d.powerOfShooter, d.factionOfShooter, d.direction, d.attackRange);
            p.transform.parent = projectileContainer.transform;
        }

    }

    public void SaveData(GameData data)
    {
        foreach (GameObject g in allCreatures)
        {
            Creature creature = g.GetComponent<Creature>();

            if (creature.currentBehaviour != BattleUnit.Behaviour.DEAD && creature.currentBehaviour != BattleUnit.Behaviour.DECEASING)
            {
                CreatureData d = creature.GetData();

                data.allCreatures.Add(d);
            }

        }

        foreach (Transform t in poopContainer.transform)
        {
            data.allPoop.Add(t.position);
        }

        foreach (GameObject o in allFood)
        {
            FoodData f = new FoodData();
            f.position = o.transform.position;
            f.foodtype = o.GetComponent<Food>().foodtype;
            f.beingConsumed = o.GetComponent<Food>().beingConsumed;

            data.allFood.Add(f);
        }

        foreach (GameObject e in allEnemies)
        {
            Enemy enemy = e.GetComponent<Enemy>();


            if (enemy.currentBehaviour != BattleUnit.Behaviour.DEAD && enemy.currentBehaviour != BattleUnit.Behaviour.DECEASING)
            {

                EnemyData d = enemy.GetData();

                data.allEnemies.Add(d);
            }


        }

        foreach (Projectile p in FindObjectsOfType(typeof(Projectile)) as Projectile[])
        {
            ProjectileData d = p.CreateSaveData();
            data.allProjectiles.Add(d);


        }


    }

    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void Unpause()
    {
        Time.timeScale = 1;
    }
}
