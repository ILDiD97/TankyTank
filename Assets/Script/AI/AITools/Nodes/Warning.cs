using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning : IBehaviourNode
{
    private AIPlayerSearch currentEnemy;

    private IBehaviourNode currentSeeSequence;

    private IBehaviourNode currentNotSeeSequence;

    public Warning(AIPlayerSearch enemy, IBehaviourNode seeSequence, IBehaviourNode notSeeSequence)
    {
        this.currentEnemy = enemy;
        this.currentSeeSequence = seeSequence;
        this.currentNotSeeSequence = notSeeSequence;
    }

    public bool Execute()
    {
        if (currentEnemy.View.CanSeePlayer)
        {
            currentSeeSequence.Execute();
            return true;
        }
        else
        {
            currentNotSeeSequence.Execute();
            return false;
        }
    }
}
