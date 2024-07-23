using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : IBehaviourNode
{
    private AIPlayerSearch enemyAI;

    private float timeToStay;

    private float timerToStay;

    private float currentTime;

    private float timeToRotate;

    private float timerToRotate;

    private float currentRotateTime;

    private bool side;

    public Patrol(AIPlayerSearch enemy, float patrolTime)
    {
        this.enemyAI = enemy;
        this.timeToStay = patrolTime;
        this.timeToRotate = patrolTime / 2;
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
                RotateTurret();
                enemyAI.SetAgentSpeed(0);
            }
            else
            {
                enemyAI.GoToLocation(enemyAI.Space.worldPos);
                currentTime = Time.time;
                currentRotateTime = Time.time;
                enemyAI.SetAgentSpeed(8);
                enemyAI.RotateToTarget(enemyAI.transform.position + 
                    enemyAI.transform.forward * 5);
            }
            return true;
        }

        return false;
    }

    private void RotateTurret()
    {
        if(timerToRotate >= currentRotateTime + timeToRotate)
        {
            currentRotateTime = Time.time;
            side = !side;
        }
        else
        {
            if (side)
            {
                enemyAI.RotateToTarget(enemyAI.transform.position
                    + enemyAI.transform.right * 5);
            }
            else 
            {
                enemyAI.RotateToTarget(enemyAI.transform.position 
                    - enemyAI.transform.right * 5);
            } 
            timerToRotate = Time.time;
        }
    }
}
