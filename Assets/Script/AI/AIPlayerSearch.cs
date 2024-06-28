using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TrueRandomInteger;

public class AIPlayerSearch : MonoBehaviour, IShootable, IFloating, ITracks
{
    [SerializeField]
    protected EnemyBaseParameter enemyParameter;

    [SerializeField] 
    private NavMeshAgent tank;

    [SerializeField]
    private FactoryBase myBase;

    [SerializeField] 
    private GameObject projectile;

    [SerializeField] 
    private GameObject player;

    [SerializeField]
    private Renderer tracks;

    [SerializeField]
    private FieldOfView view;

    [SerializeField] 
    private Transform cannon;

    [SerializeField]
    private Transform turret;

    [SerializeField]
    private Transform tankBase;

    [SerializeField]
    private AudioSource shooting;

    private CellSpace space;

    [SerializeField]
    private EAlarm alarm;

    [SerializeField]
    private Vector3 currentPosition;

    private IBehaviourNode root;

    [SerializeField]
    private float timeToShoot = 2;

    [SerializeField]
    private float projectileAccelleration = 5;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float floatingSpeed = 1;

    [SerializeField]
    private float escurtion = 0.3f;

    [SerializeField]
    private float minDistanceToShoot;

    [SerializeField]
    private int health;

    [SerializeField]
    private int enemyID = -1;

    private bool dead;

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
        IBehaviourNode shootNode = new Shooting(this, timeToShoot);
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

    public bool RotateTurret(float rotateSpeed)
    {
        float targetAngle = CalculateTargetAngle(PredictPlayerPosition());
        float currentAngle = turret.eulerAngles.y;
        float angleDelta = Mathf.DeltaAngle(currentAngle, targetAngle);
        turret.eulerAngles += new Vector3(0, angleDelta * rotateSpeed * Time.deltaTime, 0);
        return isAiming(targetAngle);
    }

    private Vector3 PredictPlayerPosition()
    {
        float timeToReach = Vector3.Distance(transform.position, player.transform.position) 
            / minDistanceToShoot;
        return View.PlayerRef.transform.position + view.PlayerSpeed * timeToReach;
    }

    private float CalculateTargetAngle(Vector3 lookPosition)
    {
        Debug.DrawLine(cannon.position, lookPosition);
        Vector3 direction = (lookPosition - turret.position).normalized;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        return angle < 0 ? 360 + angle : angle;
    }

    private bool isAiming(float angle)
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

    private CellSpace FindCell()
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

    private void Update()
    {
        if (!Dead && root != null)
        {
            BehaviourTree.Instance.TreeRootUpdate(root);
        }
    }

    //private void ChooseTask()
    //{
    //    if (View.FieldOfViewCheck(playerLayer, obstructionLayer, 50))
    //    {
    //        ShootOrFollow();
    //    }
    //    else
    //    {
    //        PatrolRandomSearch();
    //    }
    //}

    //private void ShootOrFollow()
    //{
    //    if (Vector3.Distance(transform.position, player.transform.position) < minDistance)
    //    {
    //        tank.speed = 0;
    //        transform.LookAt(player.transform);
    //    }
    //    else
    //    {
    //        tank.speed = speed;
    //        tank.SetDestination(player.transform.position);
    //    }
    //    if (!attacking && Vector3.Distance(transform.position, player.transform.position) < MinDistanceToShoot)
    //    {
    //        attacking = true;
    //        Shoot();
    //        shooting.Play();
    //    }
    //}

    //private void PatrolRandomSearch()
    //{
    //    tank.speed = speed;
    //    if (timer >= timeToMove || Vector3.Distance(transform.position, patrolPosition) < distanceFromPatrol)
    //    {
    //        Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-25,25), 
    //            0, transform.position.z + Random.Range(-25, 25));
    //        if (tank.Raycast(randomPosition, out NavMeshHit hit))
    //        {
    //            patrolPosition = hit.position;
    //            timer = 0;
    //        }
    //    }
    //    else
    //    {
    //        tank.SetDestination(patrolPosition);
    //        timer += Time.deltaTime;
    //    }
    //}

    public void Shoot()
    {
        GameObject ammo = Instantiate(projectile, cannon.position, cannon.localRotation);
        ammo.GetComponent<Rigidbody>().AddRelativeForce(cannon.forward * projectileAccelleration, ForceMode.Impulse);
        ammo.GetComponent<Projectile>().spawnerName = gameObject.name;
    }

    public void Floating()
    {
        tankBase.localPosition = new Vector3(0, Mathf.Sin(Time.time * floatingSpeed) * escurtion, 0);
    }

    public void MoveTracks(float speed)
    {
        tracks.material.SetFloat("_MaxMoveSpeed", speed / 8);
    }
}
