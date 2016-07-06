using UnityEngine;
using System.Collections;
 
[System.Serializable]
public abstract class Order : ScriptableObject{

    public abstract bool executeOrder(AIBase ai, Navigation nav);

}
