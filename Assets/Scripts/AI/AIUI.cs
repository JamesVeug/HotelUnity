using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GuestAI))]
public class AIUI : MonoBehaviour {
    private float lastTick;

    public float sleepDelay = 1f;
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
            if( sleepPrefab != null && (lastTick+sleepDelay) < Time.time )
            {
                GameObject o = GameObject.Instantiate(sleepPrefab);
                o.transform.position = transform.position + new Vector3(0,1,1);
                lastTick = Time.time;
            }
        }
	}
}
