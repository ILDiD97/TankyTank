using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TrueRandomInteger;

public class AIPlayerSearch : MonoBehaviour, IShootable, ITracks
{
    [SerializeField]
    protected EnemyBaseParameter enemyParameter;

    [SerializeField] 
    protected NavMeshAgent tank;

    [SerializeField]
    protected FactoryBase myBase;

    [SerializeField] 
    protected GameObject projectile;

    [SerializeField] 
    protected GameObject player;

    [SerializeField]
    protected Renderer tracks;

    [SerializeField]
    protected FieldOfView view;

    [SerializeField]
    protected Transform cannon;

    [SerializeField]
    protected Transform turret;

    [SerializeField]
    protected Transform tankBase;

    [SerializeField]
    protected AudioSource shooting;

    protected CellSpace space;

    [SerializeField]
    protected EAlarm alarm;

    [SerializeField]
    protected Vector3 currentPosition;

    protected IBehaviourNode root;

    [SerializeField]
    protected float projectileAccelleration = 5;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float minDistanceToShoot;

    [SerializeField]
    protected int health;

    [SerializeField]
    protected int enemyID = -1;

    protected bool dead;

    public FieldOfView View { get => view; }

    public EnemyBaseParameter EnemyParameter { get => enemyParameter; }

    public FactoryBase MyBase { get => myBase; set => myBase = value; }

    public CellSpace Space { get => space; set => space = value; }

    public EAlarm Alarm { get => alarm; set => alarm = value; }

    public Vector3 CurrentPosition { get => currentPosition; set => currentPosition = value;  }

    public int Health { get => health; set => health = value; }

    public bool Dead { get => dead; set => dead = value; }

    public float MinDistanceToShoot { get => minDistanceToShoot; }

    public int EnemyID { get => enemyID; set => enemyID = value; }

    public IBehaviourNode Root { get => root; set => root = value; }

    private void Start()
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

    public virtual void SetRootNode()
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
        IBehaviourNode shootNode = new Shooting(this, enemyParameter.TimeToShoot);
        IBehaviourNode inRange = new PlayerInRange(this, shootNode, movementNode);
        IBehaviourNode seeAlertNode = new SeePlayerDecorator(this, inRange);
        #endregion
        root = new AlertConditionalDecorator(this, 
            seeAlertNode, warning,
            soundAlertNode, patrolNode);
    }

    public bool SeePlayer()
    {
        return view.FieldOfViewCheck(enemyParameter.Enemy, enemyParameter.Obstruction, 
            enemyParameter.DistanceOfView);
    }

    public bool ControlDistance(float distance)
    {
        return Vector3.Distance(transform.position, space.worldPos) < distance;
    }

    public void GoToLocation(Vector3 loc)
    {
        tank.SetDestination(loc);
    }

    public bool IsPlayerInRange()
    {
        if (View.PlayerRef)
            return Vector3.Distance(transform.position,
                view.PlayerRef.transform.position) < minDistanceToShoot;
        else return false;
    }

    public bool RotateTurretOnPrediction()
    {
        float targetAngle = CalculateTargetAngle(PredictPlayerPosition());
        float currentAngle = turret.eulerAngles.y;
        float angleDelta = Mathf.DeltaAngle(currentAngle, targetAngle);
        //float angleBlend = Mathf.LerpAngle(turret.eulerAngles.y, angleDelta, EnemyParameter.RotateTurretSpeed);
        turret.eulerAngles += new Vector3(0, angleDelta * 
            enemyParameter.RotateTurretSpeed * Time.deltaTime, 0);
        return isAiming(targetAngle);
    }

    public void RotateToTarget(Vector3 target)
    {
        float targetAngle = CalculateTargetAngle(target);
        float currentAngle = turret.eulerAngles.y;
        //float angleBlend = Mathf.LerpAngle(turret.eulerAngles.y,
           //targetAngle, EnemyParameter.RotateTurretSpeed * Time.deltaTime);
        float angleDelta = Mathf.DeltaAngle(currentAngle, targetAngle);
        turret.eulerAngles += new Vector3(0, 
            angleDelta * enemyParameter.RotateTurretSpeed * Time.deltaTime, 0);
    }


    public Vector3 PredictPlayerPosition()
    {
        float timeToReach = Vector3.Distance(transform.position, player.transform.position) 
            / minDistanceToShoot;
        return View.PlayerRef.transform.position + view.PlayerSpeed * timeToReach;
    }

    public float CalculateTargetAngle(Vector3 lookPosition)
    {
        Debug.DrawLine(cannon.position, lookPosition);
        Vector3 direction = (lookPosition - turret.position).normalized;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        return angle < 0 ? 360 + angle : angle;
    }

    public bool isAiming(float angle)
    {
        float turretAngle = turret.eulerAngles.y < 0 ? 
            360 + turret.eulerAngles.y : turret.eulerAngles.y;
        return turretAngle  >= angle - 2 
            && turret.eulerAngles.y <= angle + 2;
    }

    public void ChooseCell()
    {
        space = FindCell();
    }

    public CellSpace FindCell()
    {
        CellSpace cell = space;
        
        List<CellSpace> activeCell = new List<CellSpace>();

        while (cell.hasFactory || cell == space)
        {
           activeCell = new List<CellSpace>();

           foreach (CellSpace space in cell.neighbors)
           {
                if (space.visited) activeCell.Add(space);
           }

            if(activeCell.Count > 0) cell = activeCell[
                TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(activeCell.Count)];
        }

        return cell;
    }

    public void SetAgentSpeed(float speed)
    {
        tank.speed = speed;
        MoveTracks(speed);
    }

    public void DeadSequence()
    {
        if (MyBase)
        {
            myBase.ReorganizeTeam(enemyID);
            myBase.SpawnTeam();
        }
        
        StartCoroutine(DestroingSequence());
    }

    protected IEnumerator DestroingSequence()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!Dead && root != null)
        {
            BehaviourTree.Instance.TreeRootUpdate(root);
        }
        else
        {
            tank.speed = 0;
            MoveTracks(0);
        }
    }

    public void Shoot()
    {
        GameObject ammo = Instantiate(projectile, cannon.position, cannon.localRotation);
        ammo.GetComponent<Rigidbody>().AddRelativeForce(cannon.forward * projectileAccelleration, ForceMode.Impulse);
        ammo.GetComponent<Projectile>().spawnerName = gameObject.name;
        shooting.PlayOneShot(shooting.clip);
    }

    //public void Floating()
    //{
    //    tankBase.localPosition = new Vector3(0, Mathf.Sin(Time.time * floatingSpeed) * escurtion, 0);
    //}

    public void MoveTracks(float speed)
    {
        if(tracks.material.GetFloat("_MaxMoveSpeed") != speed / enemyParameter.Speed)
        TrackAccelleration(speed);
    }

    protected void TrackAccelleration(float speed)
    {
        float currentAccelleration = tracks.material.GetFloat("_MaxMoveSpeed");
        if(speed == enemyParameter.Speed)
        {
            if (currentAccelleration < 1)
                currentAccelleration += Time.deltaTime * 1;
            else currentAccelleration = 1;
        }
        else
        {
            if (currentAccelleration > 0)
                currentAccelleration -= Time.deltaTime * 1;
            else currentAccelleration = 0;
        }
        tracks.material.SetFloat("_MaxMoveSpeed", currentAccelleration);
    }
}
