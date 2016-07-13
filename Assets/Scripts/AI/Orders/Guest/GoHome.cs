using UnityEngine;
using System.Collections;

public class GoHome : Order {

    private Vector2 spawnPosition = Vector2.zero;
	public override RETURN_TYPE executeOrder(AIBase ai, Navigation nav)
    {
        // Check out first if we need to
        if (ai.getOwnedRoom() != null)
        {
            ai.addOrder(ScriptableObject.CreateInstance<CheckOut>());
			return RETURN_TYPE.PROBLEM;
        }

        // Get a spawn position
        if( spawnPosition == Vector2.zero)
        {
            spawnPosition = nav.getRandomSpawnPosition();

            // Walk there
            Vector3 worldPosition = new Vector3(spawnPosition.x, 0, spawnPosition.y);
            ai.walkToPosition(worldPosition);
			return RETURN_TYPE.PROBLEM;
        }

        // Should be in reception now
		return RETURN_TYPE.COMPLETED;
    }
}
