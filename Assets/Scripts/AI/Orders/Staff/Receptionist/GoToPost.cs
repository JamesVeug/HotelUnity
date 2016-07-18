using UnityEngine;
using System.Collections;

public class GoToPost : Order {

    private Vector3 post;

    public static GoToPost create(Vector3 post)
    {
        return ScriptableObject.CreateInstance<GoToPost>().setPost(post);
    }

	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        if ((ai.transform.position-post).magnitude > 0.1)
        {
            ai.walkToPosition(post);
            return RETURN_TYPE.PROBLEM;
        }

        // Should be in reception now
        return RETURN_TYPE.COMPLETED;
    }

    public GoToPost setPost(Vector3 tile)
    {
        this.post = tile;
        return this;
    }
}
