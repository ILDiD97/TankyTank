using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;


public class UIManager : Singleton<UIManager>
{
    private TimeSpan time;

    private int kill;

    private float timeFromStart;

    [SerializeField]
    private Animation fadeOut;

    [SerializeField]
    private GameObject fade;

    public GameObject completeLevel;
    public GameObject lost;

    public TMP_Text killText;
    public TMP_Text timerText;

    public Image life;

    private void OnEnable()
    {
        ResumeTime();
    }

    public void UIScore()
    {
        kill++;
        killText.text = kill.ToString();
    }


    public void UIHealth(float health)
    {
        life.fillAmount = health / 3;
    }

    public void Win()
    {
        completeLevel.SetActive(true);
        SaveSystem.Instance.SaveIntegers("Level1Timer", time.Seconds);
        SaveSystem.Instance.SaveIntegers("Level1Kills", kill);
    }

    public void Lost()
    {
        lost.SetActive(true);
    }

    public void PauseTime()
    {
        Time.timeScale = 0;
        GameStatus.Instance.IsGameOn = false;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
        GameStatus.Instance.IsGameOn = true;
    }

    public void TimeFormStart()
    {
        if (fadeOut)
        {
            fadeOut.Play();
            StartCoroutine(StartFade());
        }
        
    }

    private IEnumerator StartFade()
    {
        yield return new WaitForSeconds(fadeOut.clip.length);
        FadeSetVisibility(false);
        time = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
        timeFromStart = Time.timeSinceLevelLoad;
        //GameStatus.Instance.SetStartGame(false);
    }

    public void FadeSetVisibility(bool visibility)
    {
        fade.SetActive(visibility);
    }

    private void Update()
    {
        if (!GameStatus.Instance.IsGameOn)
        {
            time = TimeSpan.FromSeconds(Time.timeSinceLevelLoad - timeFromStart);
            timerText.text = time.ToString("hh':'mm':'ss");
        }
    }
}
