using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioTower : MonoBehaviour
{


    public void SandPlayerLocation(Vector3 loc)
    {
        HQManager.Instance.RecieveLocation(loc);
    }
}
