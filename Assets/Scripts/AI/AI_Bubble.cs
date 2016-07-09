using UnityEngine;
using System.Collections;

public class AI_Bubble : MonoBehaviour {
    
    public float maxLife = 2;

    private float currentLife = 0;
    private float MoveSpeed = 1f;

    // Use this for initialization
    void Start () {
        transform.rotation = Camera.main.transform.rotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (currentLife > maxLife)
        {
            Destroy(gameObject);
        }

        currentLife += Time.deltaTime;

        transform.position = transform.position + transform.up * Time.deltaTime * MoveSpeed;

        // Make it smaller as its life extends
        float scale = 1-(currentLife / maxLife);
        transform.localScale = new Vector3(scale, scale, scale);


    }
}
