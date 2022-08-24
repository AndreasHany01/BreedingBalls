using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{

    public GameObject[] foodPrefabs;
    private GameManager gm;

    private const float FOOD_SPAWN_HEIGHT = 0.2f;

    public void Init(GameManager gm)
    {
        this.gm = gm;
    }

    public void SpawnFood(FoodData data)
    {

        GameObject prefab = foodPrefabs[(int) data.foodtype];

        GameObject o = Instantiate(prefab, data.position, Quaternion.identity);

        AfterSpawn(o);


        //TODO: right now the state of beingConsumed is not saved, as the creatures also
        //don't save a reference to the food.
        o.GetComponent<Food>().beingConsumed = false;
    }

    public void SpawnFood()
    {

        Vector3 spawnPosi = new Vector3(Random.Range(-6, 6), GameManager.OBJECT_HEIGHT + FOOD_SPAWN_HEIGHT, Random.Range(-6, 6));

        // Vector3 spawnPosi = new Vector3(0, GameManager.OBJECT_HEIGHT + 0.5f, 0);
        //  bool validSpawnPosi = true;

        /* do
         {
             validSpawnPosi = true;
             spawnPosi = new Vector3(Random.Range(-6, 6), OBJECT_HEIGHT + 0.5f, Random.Range(-6, 6));

             foreach (GameObject g in GameManager.allCreatures)
             {
                 Vector3 gPosi = g.transform.position;
                 if (Mathf.Round(gPosi.x) == spawnPosi.x && Mathf.Round(gPosi.z) == spawnPosi.z)
                 {
                     validSpawnPosi = false;
                     break;
                 }
             }

         } while (!validSpawnPosi); */

        GameObject o = Instantiate(foodPrefabs[Random.Range(0, foodPrefabs.Length)], spawnPosi, Quaternion.identity);
        AfterSpawn(o);
    }

    private void AfterSpawn(GameObject food)
    {

        gm.allFood.AddLast(food);

        food.transform.SetParent(gm.foodContainer.transform);
    }

}
