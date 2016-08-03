using UnityEngine;
using System.Collections;
using System;

public class TrackGuest : MonoBehaviour {

    public Vector3 offset;

    private Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if( target != null)
        {
            this.transform.position = target.position + offset;
            this.transform.LookAt(target);
        }
	}

    public void track(Transform transform)
    {
        this.target = transform;
    }

    public void stopTracking()
    {
        this.target = null;
    }
}
