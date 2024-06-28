using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : Singleton<GameStatus>
{

    [SerializeField]
    private List<AIPlayerSearch> enemies;

    [SerializeField]
    private List<FactoryBase> enemyFactories;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private int enemiesHealth;

    [SerializeField]
    private int factoriesHealth;

    [SerializeField]
    private bool gameStatus;

    public bool IsGameOn
    {
        get { return gameStatus; }
        set { gameStatus = value; }
    }

    private void Awake()
    {
        if (enemies.Count > 0 && enemyFactories.Count > 0) ClearEnemies();
    }

    public void ClearEnemies()
    {
        enemies = new List<AIPlayerSearch>();
        enemyFactories = new List<FactoryBase>();
        enemiesHealth = 0;
        factoriesHealth = 0;
    } 
    
    public void PopulateEnemies(AIPlayerSearch tanks)
    {
        enemies.Add(tanks);
    }

    public void PopulateFactory(FactoryBase factoryBase)
    {
        enemyFactories.Add(factoryBase);
    }

    public void SetPositionPlayer(Vector3 startPsition)
    {
        player.transform.position = startPsition;
        player.GetComponent<Rigidbody>().useGravity = true;
    }

    public void SetStartGame(bool active)
    {
        player.Dead = active;
        gameStatus = active;
        foreach(AIPlayerSearch game in enemies)
        {
            game.Dead = active;
        }
    }

    public void CalculateEnemyHealth()
    {
        int Health = 0;
        foreach(AIPlayerSearch enemy in enemies)
        {
            if(enemy.Health > 0)
            {
                Health += enemy.Health;
            }
        }
        enemiesHealth = Health;
        VerifyEnemyHealth();
    }

    public void CalculateFacrotyHealth()
    {
        int health = 0;
        foreach(FactoryBase factory in enemyFactories)
        {
            if(factory.Health > 0)
            {
                health += factory.Health;
            }
        }
        factoriesHealth = health;
        VerifyEnemyHealth();
    }

    public void VerifyEnemyHealth()
    {
        if(enemiesHealth <= 0 && factoriesHealth <= 0)
        {
            gameStatus = true;
            UIManager.Instance.Win();
        }
    }
}
