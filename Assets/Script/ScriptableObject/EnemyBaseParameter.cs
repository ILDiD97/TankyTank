using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Param", menuName = "New Param")]
public class EnemyBaseParameter : ScriptableObject
{
    [SerializeField]
    private LayerMask enemy;

    [SerializeField]
    private LayerMask obstruction;

    [SerializeField]
    private float distanceOfView;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float rotateTurretSpeed;

    [SerializeField]
    private float timeToShoot;


    public LayerMask Enemy { get => enemy; }
    public LayerMask Obstruction { get => obstruction; }
    public float DistanceOfView { get => distanceOfView; }
    public float Speed { get => speed; }
    public float TimeToShoot { get => timeToShoot; }
    public float RotateTurretSpeed { get => rotateTurretSpeed; }
}
