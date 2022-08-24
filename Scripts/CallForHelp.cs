using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If a creature shouts for help, a sphere collider gets created with this script attached.
/// If an allied creature is within the collider radius, its <code>ReceivingCallForHelp()</code>
/// method is being called.
/// </summary>
public class CallForHelp : MonoBehaviour
{

    public GameManager.Faction faction;
    public Vector3 positionOfHelpCaller;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(DestroyAfterTime(1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitValues(GameManager.Faction faction, Vector3 positionOfHelpCaller)
    {
        this.faction = faction;
        this.positionOfHelpCaller = positionOfHelpCaller;
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<Creature>() == null)
        {
            return;
        }

        other.GetComponent<Creature>().ReceivingCallForHelp(this);


    }
    IEnumerator DestroyAfterTime(int x)
    {
        yield return new WaitForSeconds(x);
        Destroy(gameObject);
    }
}
