using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSizer : MonoBehaviour
{

    private float baseWidth = 1920;
    private float baseHeight = 1080;

    private void Awake()
    {
        float resizeWidth = Screen.width / baseWidth;
        float resizeHeight = Screen.height / baseHeight;

        for(int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            
            child.transform.localScale = new Vector3(resizeWidth, resizeHeight);
        }
    }
}
