using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class changeGoldText : MonoBehaviour {
    private float maxLife = 0.5f;
    private float currentLife = 0;

    private float moveAngle;
    private float MoveSpeed = 0.5f;
    private float startTime;
    private Text text;

    // Use this for initialization
    void Start () {
        startTime = Time.time;
        text = GetComponent<Text>();
        moveAngle = Random.Range(-1f, 1f);
	}
	
	// Update is called once per frame
	void Update () {

        if (currentLife > maxLife)
        {
            Destroy(gameObject);
        }

        currentLife += Time.deltaTime;

        Vector3 change = Vector3.zero;
        change -= transform.up * MoveSpeed; // Move down
        change += transform.right * moveAngle * MoveSpeed;

        transform.position = transform.position + change;
        text.color = new Color(text.color.r, text.color.g, text.color.b, (1 - (currentLife / maxLife)));
	}
}
