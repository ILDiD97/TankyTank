using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInRange : IBehaviourNode
{
    private AIPlayerSearch currentEnemy;

    private IBehaviourNode currentShoot;

    private IBehaviourNode currentMovement;



    public PlayerInRange(AIPlayerSearch enemy, IBehaviourNode shoot, IBehaviourNode movement)
    {
        this.currentEnemy = enemy;
        this.currentShoot = shoot;
        this.currentMovement = movement;
    }

    public bool Execute()
    {
        if (currentEnemy.IsPlayerInRange())
        {
            currentShoot.Execute();
            currentEnemy.SetAgentSpeed(0);
        }
        else
        {
            if(currentEnemy.View.PlayerRef)
                currentEnemy.RotateToTarget(
                    currentEnemy.View.PlayerRef.transform.position);
            currentEnemy.SetAgentSpeed(8);
            currentMovement.Execute();
        }
        return false;
    }
}
