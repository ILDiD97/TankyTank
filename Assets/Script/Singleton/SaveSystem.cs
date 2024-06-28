using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : Singleton<SaveSystem>
{
    public void SaveIntegers(string levelNameValue, int value)
    {
        PlayerPrefs.SetInt(levelNameValue, value);
    }

    public void SaveFloats(string levelNameValue, float value)
    {
        PlayerPrefs.SetFloat(levelNameValue, value);
    }

    public void SaveStrings(string levelNameValue, string name)
    {
        PlayerPrefs.SetString(levelNameValue, name);
    }
}
