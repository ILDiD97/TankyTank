using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : IBehaviourNode
{
    private AIPlayerSearch enemyAI;

    private float timeToStay;

    private float timerToStay;

    private float currentTime;

    public Patrol(AIPlayerSearch enemy, float patrolTime)
    {
        this.enemyAI = enemy;
        this.timeToStay = patrolTime;
    }

    public bool Execute()
    {
        if (!enemyAI.View.CanSeePlayer)
        {
            if (enemyAI.ControlDistance(2))
            {
                if (timerToStay >= currentTime + timeToStay)
                {
                    enemyAI.ChooseCell();
                    currentTime = Time.time;
                }
                else
                {
                    timerToStay = Time.time;
                }
                enemyAI.SetAgentSpeed(0);
            }
            else
            {
                enemyAI.GoToLocation(enemyAI.Space.worldPos);
                currentTime = Time.time;
                enemyAI.SetAgentSpeed(8);
            }
            return true;
        }

        return false;
    }
}
