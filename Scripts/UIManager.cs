using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TMP_Text stats;
    private Creature currentTarget;
    private Camera cam;

    [SerializeField] private GameObject namesContainer;
    [SerializeField] private GameObject namePrefab;
    [SerializeField] private GameObject healthbarPrefab;
    [SerializeField] private GameObject healthbarCanvas;
    [SerializeField] public GameObject renameInputField;
    [SerializeField] public GameObject saveInputField;
    [SerializeField] public GameObject loadInputField;
    [SerializeField] private GameObject saveGameListField;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject damagePopup;

    [SerializeField] private GameObject guiArea;

    private Dictionary<int, TMP_Text> creatureNameContainer;
    private Dictionary<int, GameObject> healthbarContainer;


    private void UpdateDisplayedStats()
    {
        DisplayStats(currentTarget);
    }

    public void DisplayStats(Creature target)
    {

        //no creature selected
        if (target == null)
        {
            //no GUI clicked
            if (HideIfClickedOutside(guiArea))
            {
                //deactivate the stats GUI
                stats.transform.parent.gameObject.SetActive(false);

                //deactivate the selection circle
                DisableSelectionCircleOfPastTarget();

                //update the current target
                this.currentTarget = null;

                return;

                //GUI clicked
            }
            else
            {
                return;
            }
        }


        //deactivate the selection circle
        DisableSelectionCircleOfPastTarget();

        //update the selected target
        this.currentTarget = target;

        string gender = "M";
        if (!target.male)
        {
            gender = "F";
        }

        String hungry = "";

        if (target.hunger < 40)
        {
            hungry = "No";
        } else if (target.hunger < 80)
        {
            hungry = "Yes";
        } else
        {
            hungry = "Starving";
        }

        stats.text = target.myName //+ " " + target.id
            + "\nAge: " + (int)(target.age / 10)
            + "\n" + target.currentHp + "/" + target.MaxHp + " HP"
            + "\nPower: " + target.power
            + "\nSpeed: " + target.speed
            + "\nGender: " + gender
            + "\nFertile: " + target.breedable
            +"\nHungry: " + hungry;



        stats.transform.parent.gameObject.SetActive(true);
        target.transform.Find("SelectionCircle").gameObject.SetActive(true);
    }

    private void DisableSelectionCircleOfPastTarget()
    {
        //deactivate the selection circle of the old target
        if (this.currentTarget != null)
        {
            currentTarget.transform.Find("SelectionCircle").gameObject.SetActive(false);
        }
    }


    private bool HideIfClickedOutside(GameObject panel)
    {
        if (Input.GetMouseButton(0) && panel.activeSelf &&
            !RectTransformUtility.RectangleContainsScreenPoint(
                panel.GetComponent<RectTransform>(),
                Input.mousePosition,
                null))
        {
            //Debug.Log("not clicked on anything");
            return true;
        }

        return false;
    }




    // Start is called before the first frame update
    void Start()
    {
        stats.transform.parent.gameObject.SetActive(false);

        gameManager = GetComponent<GameManager>();
        cam = gameManager.cam;

        if (gameManager == null)
        {
            Debug.Log("no game manager found!");
        }

        InvokeRepeating("UpdateDisplayedStats", 3f, 1f);

        healthbarCanvas = GameObject.Find("CanvasHealthbars");




    }

    public void DisplayDamageNumbers(GameObject g, int damage)
    {

        GameObject o = Instantiate(damagePopup, cam.WorldToScreenPoint(g.transform.position), Quaternion.identity);
        o.transform.SetParent(healthbarCanvas.transform);
        o.GetComponent<DamagePopup>().Setup(damage);
    }

    // Update is called once per frame
    void Update()
    {


        foreach (GameObject g in gameManager.allCreatures)
        {
            //adjust name position
            TMP_Text t = creatureNameContainer[g.GetComponent<Creature>().id];
            Creature gem = g.GetComponent<Creature>();

            Vector3 screenPos = cam.WorldToScreenPoint(g.transform.position);
            t.transform.position = screenPos + new Vector3(0, 55, 0);

            //add or remove a * for the breeding status
            if (gem.breedable && !t.text.Contains("*"))
            {
                t.text += "*";
            }
            else if (!gem.breedable && t.text.Contains("*"))
            {
                t.text = t.text.Replace("*", "");
            }

            //update health bars
            UpdateHealthbars(g.GetComponent<BattleUnit>());

        }

        foreach (GameObject g in gameManager.allEnemies)
        {
            UpdateHealthbars(g.GetComponent<BattleUnit>());
        }

        // Vector3 screenPos = cam.WorldToScreenPoint(nameAnchor.transform.position);
        // nameTest.transform.position = screenPos + new Vector3(0, 25, 0);
    }

    private void UpdateHealthbars(BattleUnit b)
    {

        GameObject hpbar = healthbarContainer[b.id];
        Vector3 screenPos = cam.WorldToScreenPoint(b.transform.position);
        hpbar.transform.position = screenPos + new Vector3(0, 35, 0);
        hpbar.GetComponent<Slider>().value = b.currentHp;

        if (b.currentHp == 0)
        {
            hpbar.SetActive(false);
        } else
        {
            HealthBar hpbarscript = hpbar.GetComponent<HealthBar>();
            hpbarscript.fill.color = hpbarscript.gradient.Evaluate(hpbar.GetComponent<Slider>().normalizedValue);
        }
    }

    private void LateUpdate()
    {
    }

    public void AddCreature(GameObject g)
    {

        if (creatureNameContainer == null)
        {
            creatureNameContainer = new Dictionary<int, TMP_Text>();
        }


        //name text
        GameObject floatingNameText = Instantiate(namePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        floatingNameText.transform.SetParent(namesContainer.transform);
        TMP_Text textObject = floatingNameText.GetComponentInChildren<TMP_Text>();
        creatureNameContainer.Add(g.GetComponent<Creature>().id, textObject);

        textObject.text = g.GetComponent<Creature>().myName;

        textObject.transform.parent.gameObject.SetActive(true);


        //healthbar
        AddHealthbar(g.GetComponent<BattleUnit>());
    }

    public void AddEnemy(GameObject g)
    {

        AddHealthbar(g.GetComponent<BattleUnit>());

    }

    private void AddHealthbar(BattleUnit b)
    {
        if (healthbarContainer == null)
        {
            healthbarContainer = new Dictionary<int, GameObject>();
        }

        GameObject healthbar = Instantiate(healthbarPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        healthbar.transform.SetParent(healthbarCanvas.transform);
        healthbarContainer.Add(b.id, healthbar);
        healthbar.GetComponent<Slider>().maxValue = b.MaxHp;
        healthbar.GetComponent<Slider>().value = b.currentHp;


    }

    public void RemoveUnit(int id)
    {
        if (creatureNameContainer.ContainsKey(id))
        {

            TMP_Text t = creatureNameContainer[id];
            Destroy(t);
            creatureNameContainer.Remove(id);
        }

        if (healthbarContainer.ContainsKey(id))
        {

            GameObject healthbar = healthbarContainer[id];
            Destroy(healthbar);
            healthbarContainer.Remove(id);
        }
    }

    public void RenameButton()
    {
        renameInputField.transform.gameObject.SetActive(true);
        renameInputField.GetComponent<InputField>().Select();
        renameInputField.GetComponent<InputField>().ActivateInputField();
        //gameManager.ignoreClickingDefaultBehavourForThisTick = true;
        //Debug.Log("clicked");
    }
    public void Rename()
    {

        Debug.Log(currentTarget.myName);
        currentTarget.myName = renameInputField.GetComponent<InputField>().text;
        creatureNameContainer[currentTarget.id].text = currentTarget.myName;

        //hide rename input field
        renameInputField.transform.gameObject.SetActive(false);

        //update the displayed stats
        DisplayStats(currentTarget);
    }

    public void SaveButton()
    {

        if (saveInputField.transform.gameObject.activeSelf)
        {
            saveInputField.transform.gameObject.SetActive(false);
            gameManager.Unpause();
            return;
        }


        saveInputField.transform.gameObject.SetActive(true);
        saveInputField.GetComponent<InputField>().Select();
        saveInputField.GetComponent<InputField>().ActivateInputField();
        //saveInputField.GetComponent<InputField>().text = "Save " + DateTime.Now;
        saveInputField.GetComponent<InputField>().text = "Save_" + UnityEngine.Random.Range(0, 100);
        gameManager.Pause();
    }

    public void Save()
    {

        string saveName = saveInputField.GetComponent<InputField>().text;
        Debug.Log(saveName);
        saveInputField.transform.gameObject.SetActive(false);
        gameManager.Save(saveName);
        gameManager.Unpause();

    }


    public void LoadButton()
    {
        if (loadInputField.transform.gameObject.activeSelf)
        {

            loadInputField.transform.gameObject.SetActive(false); 
            saveGameListField.SetActive(false);
            return;
        }

        loadInputField.transform.gameObject.SetActive(true);
        loadInputField.GetComponent<InputField>().Select();
        loadInputField.GetComponent<InputField>().ActivateInputField();
        loadInputField.GetComponent<InputField>().text = "";

        string allFileNames = DataPersistenceManager.instance.GetAllSaveFileNamesAsString();
        string toDisplay = allFileNames.Replace(", ", "\n").Replace(".game", "");
        if (!toDisplay.Equals(""))
        {
            toDisplay = "Save games found: \n" + toDisplay;

            loadInputField.GetComponent<InputField>().text = toDisplay.Split("\n")[1];

        } else
        {
            toDisplay = "No save games found";
            loadInputField.transform.gameObject.SetActive(false);

        }
        saveGameListField.GetComponent<Text>().text = toDisplay;
        saveGameListField.SetActive(true);
    }


    public void Load()
    {

        saveGameListField.SetActive(false);
        string saveName = loadInputField.GetComponent<InputField>().text;
        Debug.Log("trying to load " + saveName);
        loadInputField.transform.gameObject.SetActive(false);
        gameManager.Load(saveName);

    }
}