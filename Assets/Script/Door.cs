using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Door : MonoBehaviour
{
    
    [SerializeField]
    Transform firstDoor;

    [SerializeField]
    Transform secondDoor;

    int iterations;
    // Start is called before the first frame update
    void Start()
    {
        StartOpen();
    }

    private void StartOpen()
    {
        Destroy(firstDoor.gameObject);
        Destroy(secondDoor.gameObject);
    }

    private IEnumerator Open()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        firstDoor.localPosition = new Vector3(firstDoor.localPosition.x, firstDoor.localPosition.y, firstDoor.localPosition.z - Time.deltaTime * 10);
        secondDoor.localPosition = new Vector3(secondDoor.localPosition.x, secondDoor.localPosition.y, secondDoor.localPosition.z + Time.deltaTime * 10);
        if(iterations < 10)
        {
            iterations++;
            StartOpen();
        }
        else
        {
            firstDoor.gameObject.SetActive(false);
            secondDoor.gameObject.SetActive(false);
        }
    }
}
