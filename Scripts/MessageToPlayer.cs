using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageToPlayer : MonoBehaviour
{
    private const float DISPLAY_MESSAGE_FOR_X_SECONDS = 4f;

    void Start()
    {
        this.gameObject.GetComponent<TMP_Text>().text = "";
    }

    public void DisplayMessageToPlayer(string s)
    {
        this.gameObject.GetComponent<TMP_Text>().text = s;

        StartCoroutine(RemoveMessage());
    }

    IEnumerator RemoveMessage()
    {
        yield return new WaitForSeconds(DISPLAY_MESSAGE_FOR_X_SECONDS);
        this.gameObject.GetComponent<TMP_Text>().text = "";
    }
}
