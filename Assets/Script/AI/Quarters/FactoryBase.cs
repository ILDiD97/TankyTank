using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueRandomInteger;

public class FactoryBase : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private List<AIPlayerSearch> myTeamMate;

    [SerializeField]
    private EAlarm alarm;

    [SerializeField]
    private Vector3 playerLocation;

    [SerializeField]
    private float timerControlAlert;

    [SerializeField]
    private float timeAlert;

    [SerializeField]
    private float warnLimit;

    [SerializeField]
    private float warnCounter;

    [SerializeField]
    private int health;

    [SerializeField]
    private int teamLimit;

    [SerializeField]
    private int factoryID;

    [SerializeField]
    private int warnEnemyID;

    public CellSpace ownCell;

    public List<AIPlayerSearch> MyTeamMate { get => myTeamMate; }

    public Vector3 PlayerLocation { get => playerLocation; }
    
    public EAlarm Alarm { get => alarm; set => alarm = value; }

    public int Health { get => health; set => health = value; }

    public int FactoryID { get => factoryID; set => factoryID = value; }

    public int WarnEnemyID { get => warnEnemyID; set => warnEnemyID = value; }

    public void SpawnTeam()
    {
        if(MyTeamMate.Count < teamLimit)
        {
            StartCoroutine(SpawnTeamMate(5));
        }
    }

    public IEnumerator SpawnTeamMate(int waitToSpawn)
    {
        int wait = 3;
        yield return new WaitForSeconds(wait);
        if (health > 0)
        {
            Vector3 location = new Vector3(transform.position.x - 5, transform.position.y, transform.position.z);
            int cellLocation = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(ownCell.obstaclePositions.Count);
            GameObject mate = Instantiate(enemyPrefab, location, Quaternion.identity);

            AIPlayerSearch currentMate = mate.GetComponent<AIPlayerSearch>();
            currentMate.Space = ownCell;
            currentMate.MyBase = this;
            currentMate.EnemyID = myTeamMate.Count;
            myTeamMate.Add(currentMate);

            GameStatus.Instance.PopulateEnemies(currentMate);
            GameStatus.Instance.CalculateEnemyHealth();

            yield return new WaitForSeconds(waitToSpawn - wait);

            SpawnTeam();

        }

    }

    public void GetPlayerPosition(int teamMateID)
    {
        playerLocation = MyTeamMate[teamMateID].View.PlayerRef.transform.position;
    }

    public void ReorganizeTeam(int id)
    {
        MyTeamMate.RemoveAt(id);
        foreach(AIPlayerSearch enemy in MyTeamMate)
        {
            if (enemy.EnemyID > id) enemy.EnemyID--;
        }
    }

    public void AlertTeamMate(EAlarm alert)
    {
        foreach(AIPlayerSearch search in MyTeamMate)
        {
            search.Alarm = alert;
        }
    }

    public void DestoryTeamMate()
    {
        foreach(AIPlayerSearch enemy in MyTeamMate)
        {
            DestroyImmediate(enemy);
        }
        myTeamMate = new List<AIPlayerSearch>();
    }

    private void Update()
    {
        if(alarm != EAlarm.SeeAlert)
        {
            ControlAlertState();
        }
        else
        {
            WaitToContorl();
        }
        
    }

    private void WaitToContorl()
    {
        if(timerControlAlert >= timeAlert)
        {
            ControlAlertState();
            timerControlAlert = 0;
        }
        else
        {
            timerControlAlert += 1 * Time.deltaTime;
            EvaluateAlert();
        }
    }

    private void ControlAlertState()
    {
        if (EvaluateAlert())
        {
            AlertState();
        }
        else
        {
            alarm = EAlarm.Patrol;
        }
    }

    private bool EvaluateAlert()
    {
        foreach(AIPlayerSearch mate in myTeamMate)
        {
            if (mate.Alarm == EAlarm.SeeAlert && mate.View.PlayerRef) 
            {
                playerLocation = mate.View.PlayerRef.transform.position;
                return true; 
            }
        }

        return false;
    }

    private void AlertState()
    {
        if (ControlAlarmLimit())
        {
            alarm = EAlarm.SeeAlert;
            TeamWarning();
        }
        else
        {
            IncreseLevelWarn();
        }
    }

    private void TeamWarning()
    {
        foreach(AIPlayerSearch teamMate in MyTeamMate)
        {
            if (teamMate.Alarm != EAlarm.SeeAlert)
            {
                teamMate.Alarm = EAlarm.Warning;
            }
        }
    }

    private bool ControlAlarmLimit()
    {
        return warnCounter >= warnLimit;
    }

    private void IncreseLevelWarn()
    {
        warnCounter += 0.1f * HQManager.Instance.WarnMultiplier;
    }
}
