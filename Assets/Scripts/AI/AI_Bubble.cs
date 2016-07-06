using UnityEngine;
using System.Collections;

public class AI_Bubble : MonoBehaviour {
    
    public float maxLife = 2;

    private float currentLife = 0;
    private float MoveSpeed = 1f;
    private float frequency = 5.0f;  // Speed of sine movement
    private float magnitude = 0.2f;   // Size of sine movement
    private Vector3 axis;

    private Vector3 pos;

    // Use this for initialization
    void Start () {
        transform.rotation = Camera.main.transform.rotation;
        pos = transform.position;
        axis = transform.right;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (currentLife > maxLife)
        {
            Destroy(gameObject);
        }

        currentLife += Time.deltaTime;

        pos += transform.up * Time.deltaTime * MoveSpeed;
        transform.position = pos + axis * Mathf.Sin(Time.time * frequency) * magnitude;

    }
}
