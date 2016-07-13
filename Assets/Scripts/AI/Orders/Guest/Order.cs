using UnityEngine;
using System.Collections;
 
[System.Serializable]
public abstract class Order : ScriptableObject{

	public enum RETURN_TYPE{
		COMPLETED,
		PROBLEM,
		FAILED
	}

	public static RETURN_TYPE toReturnType(bool b){
		return b ? RETURN_TYPE.COMPLETED : RETURN_TYPE.FAILED;
	}

	public static bool toBool(RETURN_TYPE t){
		if (t == RETURN_TYPE.COMPLETED)
			return true;
		if (t == RETURN_TYPE.PROBLEM)
			return false;
        if (t == RETURN_TYPE.FAILED)
            return false;

        Debug.LogError ("Can not convert type " + t);
		return false;
	}

	public abstract RETURN_TYPE executeOrder(AIBase ai, Navigation nav);

}
