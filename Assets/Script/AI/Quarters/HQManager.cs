using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueRandomInteger;

public class HQManager : Singleton<HQManager>
{
    [SerializeField]
    private List<GameObject> factoryPrefabs;

    [SerializeField]
    private List<FactoryBase> factories;

    [SerializeField]
    private EAlarm alarm;

    [SerializeField]
    private Vector3 playerLocation;

    [SerializeField]
    private int factoryLimit;

    [SerializeField]
    private int limitFactoryParam;

    [SerializeField]
    private float warnMultiplier;


    public int FactoryLimit { get => factoryLimit; set => factoryLimit = value; }

    public int LimitFactoryParam { get => limitFactoryParam; }

    public float WarnMultiplier { get => warnMultiplier; }

    public void SpawnFactory(CellSpace cell)
    {
        GameObject currentFactory = Instantiate(factoryPrefabs[TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(factoryPrefabs.Count)]);
        currentFactory.transform.position = new Vector3(cell.worldPos.x, 0, cell.worldPos.z);

        FactoryBase currentFactoryBase = currentFactory.GetComponent<FactoryBase>();
        currentFactoryBase.ownCell = cell;
        currentFactoryBase.FactoryID = factories.Count;
        factories.Add(currentFactoryBase);
        currentFactoryBase.SpawnTeam();

        GameStatus.Instance.PopulateFactory(currentFactoryBase);
        GameStatus.Instance.CalculateFacrotyHealth();
    }
    
    public void RecieveLocation(Vector3 location)
    {
        playerLocation = location;
    }

    public void DestoryFactory()
    {
        foreach(FactoryBase factory in factories)
        {
            factory.DestoryTeamMate();
            DestroyImmediate(factory);
        }
        factories = new List<FactoryBase>();
    }
}
