using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        
    }

    public void AdjustTiling(float x, float y)
    {

        this.gameObject.GetComponent<Renderer>().material.mainTextureScale = new Vector2(x, y);
    }
}
