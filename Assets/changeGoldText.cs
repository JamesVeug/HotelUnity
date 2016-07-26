using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class changeGoldText : MonoBehaviour {
    private float maxLife = 0.5f;
    private float currentLife = 0;

    private float MoveSpeed = 0.5f;
    private float startTime;
    private Text text;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {

        if (currentLife > maxLife)
        {
            Destroy(gameObject);
        }

        currentLife += Time.deltaTime;

        transform.position = transform.position - transform.up * MoveSpeed;
        text.color = new Color(text.color.r, text.color.g, text.color.b, (1 - (currentLife / maxLife)));
	}
}
