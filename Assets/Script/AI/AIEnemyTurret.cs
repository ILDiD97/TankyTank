using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TrueRandomInteger;

public class AIEnemyTurret : AIPlayerSearch
{

    // Start is called before the first frame update
    void Start()
    {
        tracks.material = new Material(tracks.material);
        tankBase = transform.GetChild(0);
        tank = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        speed = tank.speed;
        space = myBase.ownCell;
        ChooseCell();
        SetRootNode();
    }

    public override void SetRootNode()
    {
        #region "Patrol"        
        IBehaviourNode patrolNode = new Patrol(this, 10);
        #endregion
        #region "SoundAlert"
        IBehaviourNode soundAlertNode = new Sequence();
        #endregion
        #region "Warning"
        IBehaviourNode warning = new MoveToAdvice(this);
        #endregion
        #region "SeeAlert"
        IBehaviourNode movementNode = new Movement(this);
        IBehaviourNode shootNode = new Shooting(this, EnemyParameter.TimeToShoot);
        IBehaviourNode inRange = new PlayerInRange(this, shootNode, movementNode);
        IBehaviourNode seeAlertNode = new SeePlayerDecorator(this, inRange);
        #endregion
        Root = new AlertConditionalDecorator(this,
            seeAlertNode, warning,
            soundAlertNode, patrolNode);
    }



    // Update is called once per frame
    void Update()
    {
        if (!Dead && Root != null)
        {
            BehaviourTree.Instance.TreeRootUpdate(Root);
        }
    }
}
