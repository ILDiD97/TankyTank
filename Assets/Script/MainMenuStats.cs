using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MainMenuStats : MonoBehaviour
{
    public TMP_Text level1;
    // Start is called before the first frame update
    void Start()
    {
        level1.text = "Timer " + PlayerPrefs.GetInt("Level1Timer") + "\nKill " + PlayerPrefs.GetInt("Level1Kills"); 
    }

}
