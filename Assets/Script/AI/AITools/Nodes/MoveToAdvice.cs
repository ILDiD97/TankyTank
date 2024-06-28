using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToAdvice : IBehaviourNode
{
    private AIPlayerSearch currentEnemy;

    public MoveToAdvice(AIPlayerSearch enemy)
    {
        this.currentEnemy = enemy;
    }

    public bool Execute()
    {
        currentEnemy.GoToLocation(currentEnemy.MyBase.PlayerLocation);
        return true;
    }

}
