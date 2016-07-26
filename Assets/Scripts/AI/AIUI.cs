using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GuestAI))]
public class AIUI : MonoBehaviour {
    private float lastTick;
    private Gold goldReference;

    public float sleepDelay = 1f;
    public GameObject sleepPrefab;
    public GameObject gainMoneyPrefab;
    public GameObject loseMoneyPrefab;

    private GuestAI ai;
	// Use this for initialization
	void Start () {
        ai = GetComponent<GuestAI>();
        goldReference = Gold.create(ai.gold.amount);
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
                AI_Bubble bubble = o.GetComponent<AI_Bubble>();
                bubble.initialWorldPosition = ai.transform.position + new Vector3(1,2,0);
                o.transform.SetParent(GameLogic.FindObjectOfType<Canvas>().transform);
                lastTick = Time.time;
            }
        }

        // Check if our gold amount of changed
        if( goldReference != ai.gold)
        {
            goldReference.amount = ai.gold.amount;

            GameObject o = null;
            if (goldReference < ai.gold)
            {
                o = GameObject.Instantiate(gainMoneyPrefab);
            }
            else
            {
                o = GameObject.Instantiate(loseMoneyPrefab);
            }
            AI_Bubble bubble = o.GetComponent<AI_Bubble>();
            bubble.initialWorldPosition = ai.transform.position + new Vector3(0, 2, 0);
            o.transform.SetParent(GameLogic.FindObjectOfType<Canvas>().transform);

            //o.transform.position = transform.position + new Vector3(0, 1, 1);
        }
	}
}
