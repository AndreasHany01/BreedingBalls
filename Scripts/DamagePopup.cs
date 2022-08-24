using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private const float START_TO_DISAPPEAR_AFTER = 1f;
    private const float DISAPPEAR_DURATION = 3f;
    private const float MOVE_SPEED = 30f;
    private TextMeshProUGUI textMesh;
    private float startToDisappearTimer;
    private Color textColor;

    public void Setup(int damageAmount)
    {
        if (textMesh == null)
        {

            textMesh = transform.GetComponent<TextMeshProUGUI>();
        }

        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        startToDisappearTimer = START_TO_DISAPPEAR_AFTER;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, MOVE_SPEED, 0) * Time.deltaTime;

        startToDisappearTimer -= Time.deltaTime;

        if (startToDisappearTimer < 0)
        {
            textColor.a -= DISAPPEAR_DURATION * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }

        }
    }
}
