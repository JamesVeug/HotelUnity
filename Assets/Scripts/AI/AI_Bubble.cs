using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AI_Bubble : MonoBehaviour {
    
    public float maxLife = 2;

    private float currentLife = 0;
    private float MoveSpeed = 1f;

    public Vector3 initialWorldPosition;
    private Vector3 initialSize;

    // Use this for initialization
    void Start () {
        transform.rotation = Quaternion.identity;
        initialSize = gameObject.transform.localScale;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (currentLife > maxLife)
        {
            Destroy(gameObject);
        }

        currentLife += Time.deltaTime;

        initialWorldPosition += transform.up * Time.deltaTime * MoveSpeed;
        transform.position = Camera.main.WorldToScreenPoint(initialWorldPosition);

        // Make it smaller as its life extends
        //transform.localScale = (1-(currentLife / maxLife))* initialSize;
        //transform.localScale = new Vector3(scale, scale, scale);


        changeImageAlpha(transform);
    }

    private void changeImageAlpha(Transform transform)
    {
        Image image = transform.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, (1 - (currentLife / maxLife)));
        }

        foreach(Transform t in transform)
        {
            changeImageAlpha(t);
        }
    }
}
