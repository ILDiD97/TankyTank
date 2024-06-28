using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntrancesBehevior : MonoBehaviour
{
    [SerializeField]
    private GameObject entranceDoor;

    [SerializeField]
    private GameObject entranceWall;

    public void SelectEntranceActive(bool active, bool isEditor)
    {
        if (!isEditor)
        {
            if (active)
            {
                Destroy(entranceWall);
            }
            else
            {
                Destroy(entranceDoor);
            }
        }
        else
        {
            if (active)
            {
                DestroyImmediate(entranceWall);
            }
            else
            {
                DestroyImmediate(entranceDoor);
            }
        }
    }

    public IEnumerator DestoryRightChild(int index)
    {
        yield return new WaitForSeconds(6);
        Destroy(transform.GetChild(index).gameObject);
    }

}
