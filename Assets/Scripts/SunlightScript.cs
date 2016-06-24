using UnityEngine;
using System.Collections;

public class SunlightScript : MonoBehaviour {

    public Vector3 rotationSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 rot = transform.rotation.eulerAngles;
        rot += rotationSpeed;

        transform.rotation = Quaternion.Euler(rot);
	}
}
