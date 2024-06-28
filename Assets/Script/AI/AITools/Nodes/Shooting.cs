using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : IBehaviourNode
{
    private AIPlayerSearch aiPlayerSearch;

    private float shootingTime;

    private float timerToShoot;

    private float currentTime;

    public Shooting(AIPlayerSearch playerSearch, float timeToShoot)
    {
        this.aiPlayerSearch = playerSearch;
        this.shootingTime = timeToShoot;
    }

    public bool Execute()
    {
        if(aiPlayerSearch.RotateTurret(30))
        {
            if(timerToShoot >= currentTime + shootingTime)
            {
                aiPlayerSearch.Shoot();
                currentTime = Time.time;
                return true;
            }
            else
            {
                timerToShoot = Time.time;
                return false;
            }
        }
        else
        {
            currentTime = Time.time;
        }
        return false;


    }

}
