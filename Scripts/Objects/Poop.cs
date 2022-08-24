using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poop : MonoBehaviour
{
    private bool decease = false;
    private AudioSource audioSource;
    public bool loadedFromSave = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterTime(20));
        audioSource = GetComponent<AudioSource>();

        if (Random.Range(0,2) == 0 && !loadedFromSave)
        {
            audioSource.volume = AudioManager.soundVolume;
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (decease)
        {
            transform.Translate(0.3f * Time.deltaTime * new Vector3(0,-1,0));

            if (this.transform.position.y < -3)
            {

                Destroy(gameObject);
            }
        }

    }
    IEnumerator DestroyAfterTime(int x)
    {
        yield return new WaitForSeconds(x);
        decease = true;
    }


}
