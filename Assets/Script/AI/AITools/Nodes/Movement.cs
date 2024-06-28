using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : IBehaviourNode
{
    private AIPlayerSearch aiPlayerSearch;

    public Movement(AIPlayerSearch playerSearch)
    {
        this.aiPlayerSearch = playerSearch;
    }

    public bool Execute()
    {
        if (aiPlayerSearch.View.CanSeePlayer && aiPlayerSearch.View.PlayerRef)
        {
            aiPlayerSearch.GoToLocation(aiPlayerSearch.View.PlayerRef.transform.position);
            return true;
        }
        return false;
    }

}
