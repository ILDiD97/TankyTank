using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectFollower : Singleton<SceneObjectFollower>
{
    [SerializeField]
    private static GameObject target; // Il GameObject da seguire

    [SerializeField]
    private static bool canBeUpdated;
}
