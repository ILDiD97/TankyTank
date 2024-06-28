using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    
    [SerializeField]
    [Range(0,360)]
    private float angle = 90f;

    [SerializeField]
    private GameObject playerRef;

    [SerializeField]
    private Vector3 playerSpeed;

    private Vector3 previousPosition;

    private Vector3 currentPosition;

    [SerializeField]
    private Collider[] rangeChecks;

    [SerializeField]
    private Transform cannon;

    [SerializeField]
    private bool canSeePlayer;

    [SerializeField]
    private bool canSeeDebug;


    public bool CanSeePlayer { get => canSeePlayer; }

    public GameObject PlayerRef { get => playerRef; }

    public Vector3 PlayerSpeed { get => playerSpeed; }

    public bool FieldOfViewCheck(LayerMask enemyMask, LayerMask obstructionMask, float distance)
    {
        rangeChecks = Physics.OverlapSphere(transform.position, distance, enemyMask);

        canSeePlayer = false;

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (new Vector3(target.position.x, cannon.position.y, target.position.z) -
                cannon.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(cannon.position, target.position);

                if (!Physics.Raycast(cannon.position, directionToTarget,
                    out RaycastHit hit, distanceToTarget, obstructionMask))
                {
                    // If the raycast doesn't hit anything, the AI can see the player
                    PlayerStats(target);                    
                }
                else
                {
                    // If the raycast hits something, check if it's the player
                    if (hit.transform.gameObject.layer == enemyMask)
                    {
                        PlayerStats(target);                        
                    }

                    playerRef = null;
                }
            }
        }

        return CanSeePlayer;
    }

    private void PlayerStats(Transform player)
    {
        previousPosition = currentPosition ;
        canSeePlayer = true;
        playerRef = player.gameObject;
        currentPosition = player.position;
        playerSpeed = (currentPosition - previousPosition) / Time.deltaTime;
        if (canSeeDebug)
        {
            Debug.DrawLine(cannon.position, playerRef.transform.position + 
                playerSpeed * Vector3.Distance(cannon.position, playerRef.transform.position));
        }
    }

    //void OnDrawGizmos()
    //{
    //    if (canSeeDebug)
    //    {
    //        Gizmos.color = Color.red; // adjust the color to your liking
    //        Gizmos.DrawWireSphere(transform.position, 50); // draw the sphere

    //        if (CanSeePlayer)
    //        {
    //            Gizmos.color = Color.green; // adjust the color to your liking
    //            Gizmos.DrawLine(cannon.position, rangeChecks[0].transform.position); // draw a line to the target
    //        }
    //        //else
    //        //{
    //        //    Gizmos.color = Color.green; // adjust the color to your liking
    //        //    Gizmos.DrawLine(cannon.position, cannon.forward); // draw a line to the target
    //        //}

    //        // draw the FOV cone
    //        Gizmos.color = Color.yellow; // adjust the color to your liking
    //        Vector3 fovDirection = transform.forward;
    //        Vector3 fovLeft = Quaternion.Euler(0, -angle / 2, 0) * fovDirection;
    //        Vector3 fovRight = Quaternion.Euler(0, angle / 2, 0) * fovDirection;
    //        Gizmos.DrawLine(cannon.position, cannon.position + fovLeft * 50);
    //        Gizmos.DrawLine(cannon.position, cannon.position + fovRight * 50);
    //        Gizmos.DrawLine(cannon.position + fovLeft * 50, cannon.position + fovRight * 50);

    //    }
    //}
}
