using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCameraScript : MonoBehaviour
{

    private int speed = 10;
    private int zoomSpeed = 110;
    private GameManager gameManager;

    public void Init(GameManager gm)
    {
        this.gameManager = gm;
    }

    void Update()
    {

        UIManager uim = gameManager.uimanager;

        //disable camera movement if user is typing into a field
        if (uim.loadInputField.activeSelf || uim.saveInputField.activeSelf || uim.renameInputField.activeSelf )
        {
            return;
        }

        if (Input.GetKey(KeyCode.S))
        {

            transform.Translate(speed * Time.deltaTime * new Vector3(0,-1,0));
        } else if (Input.GetKey(KeyCode.W))
        {

            transform.Translate(speed * Time.deltaTime * new Vector3(0, 1, 0));
        }
        else if (Input.GetKey(KeyCode.A))
        {

            transform.Translate(speed * Time.deltaTime * new Vector3(-1, 0, 0));
        }
        else if (Input.GetKey(KeyCode.D))
        {

            transform.Translate(speed * Time.deltaTime * new Vector3(1, 0, 0));
        }

    }

    void OnGUI()
    {
        if (Input.mouseScrollDelta.y > 0)
        {

            transform.Translate(zoomSpeed * Time.deltaTime * new Vector3(0, 0, 1));
        } else if (Input.mouseScrollDelta.y < 0)
        {

            transform.Translate(zoomSpeed * Time.deltaTime * new Vector3(0, 0, -1));
            //Debug.Log(zoomSpeed + ", " + Time.deltaTime);
        }
    }
}
