using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeePlayerDecorator : IBehaviourNode
{
    private AIPlayerSearch currentEnemy;

    private IBehaviourNode currentFirstNode;


    public SeePlayerDecorator(AIPlayerSearch enemy,IBehaviourNode firstNode)
    {
        this.currentEnemy = enemy;
        this.currentFirstNode = firstNode;
    }

    public bool Execute()
    {
        currentFirstNode.Execute();
        return true;
    }

}
