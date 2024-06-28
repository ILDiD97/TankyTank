using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDecorator : IBehaviourNode
{
    private AIPlayerSearch currentEnemy;

    private IBehaviourNode currentLiveNode;

    private IBehaviourNode currentDeathNode;

    public DeathDecorator(AIPlayerSearch enemy, IBehaviourNode liveNode, IBehaviourNode deathNode)
    {
        this.currentEnemy = enemy;
        this.currentLiveNode = liveNode;
        this.currentDeathNode = deathNode;
    }

    public bool Execute()
    {
        if (!currentEnemy.Dead)
        {
            currentLiveNode.Execute();
            return true;
        }
        else
        {
            currentDeathNode.Execute();
            return false;
        }
    }
}
