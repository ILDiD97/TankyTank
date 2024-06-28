using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertConditionalDecorator : IBehaviourNode
{
    private AIPlayerSearch enemyAI;

    private IBehaviourNode seeAlert;

    private IBehaviourNode warningAlert;

    private IBehaviourNode soundAlert;

    private IBehaviourNode patrol;

    public AlertConditionalDecorator(AIPlayerSearch enemy, IBehaviourNode seeAlertNode, 
        IBehaviourNode warningNode, IBehaviourNode soundAlertNode, 
        IBehaviourNode patrolNode)
    {
        this.enemyAI = enemy;
        this.seeAlert = seeAlertNode;
        this.warningAlert = warningNode;
        this.soundAlert = soundAlertNode;
        this.patrol = patrolNode;
    }
    public bool Execute()
    {
        enemyAI.Alarm = ControlAlert();

        switch (enemyAI.Alarm)
        {
            case EAlarm.SeeAlert:
                return seeAlert.Execute();
            case EAlarm.Warning:
                return warningAlert.Execute();
            case EAlarm.SoundAlert:
                return soundAlert.Execute();
            default:
                return patrol.Execute();
        }
    }

    private EAlarm ControlAlert()
    {
        if (enemyAI.SeePlayer())
        {
            return EAlarm.SeeAlert;
        }
        else if (enemyAI.MyBase.Alarm == EAlarm.SeeAlert) return EAlarm.Warning;
        else return EAlarm.Patrol;
    }
}
