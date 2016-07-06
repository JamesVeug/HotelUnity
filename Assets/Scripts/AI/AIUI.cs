using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GuestAI))]
public class AIUI : MonoBehaviour {
    public GameObject sleepPrefab;

    private GuestAI ai;
	// Use this for initialization
	void Start () {
        ai = GetComponent<GuestAI>();
	}
	
	// Update is called once per frame
	void Update () {

        Order order = ai.getCurrentOrder();
        if( order == null)
        {
            return;
        }

        if( order is Sleep)
        {
            if( sleepPrefab != null)
            {
                GameObject o = GameObject.Instantiate(sleepPrefab);
                o.transform.position = transform.position + new Vector3(0,2.5f,0);
            }
        }
	}
}
