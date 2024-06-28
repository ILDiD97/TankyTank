using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallFactory : IBehaviourNode
{
    private AIPlayerSearch currentEnemy;

    public CallFactory(AIPlayerSearch enemy)
    {
        this.currentEnemy = enemy;
    }

    public bool Execute()
    {
        if(EvaluateCall())
        {
            currentEnemy.MyBase.WarnEnemyID = currentEnemy.EnemyID;
            //currentEnemy.Alarm = EAlarm.SeeAlert;
            //currentEnemy.MyBase.WarnRadio(currentEnemy.EnemyID, currentEnemy.Alarm);
        }
        else if(currentEnemy.EnemyID == currentEnemy.MyBase.WarnEnemyID)
        {
            //currentEnemy.Alarm = EAlarm.SeeAlert;
            //currentEnemy.MyBase.WarnRadio(currentEnemy.EnemyID, currentEnemy.Alarm);
        }
        
        return true;
    }

    private bool EvaluateCall()
    {
        foreach (AIPlayerSearch teamMate in currentEnemy.MyBase.MyTeamMate)
        {
            if (teamMate.Alarm == EAlarm.SeeAlert) return false;
        }
        return true;
    }


}
