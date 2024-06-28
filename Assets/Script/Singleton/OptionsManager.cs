using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] private TMP_Text soundEffectSlideVolumeText;
    [SerializeField] private TMP_Text musicSlideVolumeText;
    [SerializeField] private AudioMixer SoundEffectMixer;
    [SerializeField] private AudioMixer MusicMixer;

    private float soundEffectsVolume;
    private float musicVolume;


    public void SoundEffectSlideVolumeChange(float value)
    {
        soundEffectsVolume = value;
        soundEffectSlideVolumeText.text = ((int)value + 80).ToString();
        SoundEffectMixerChange();
    }
    private void SoundEffectMixerChange()
    {
        SoundEffectMixer.SetFloat("SoundEffectVolume", soundEffectsVolume);
    }

    public void MusicSlideVolumeChange(float value)
    {
        musicVolume = value;
        musicSlideVolumeText.text = ((int)value + 80).ToString();
        MusicMixerChange();
    }

    private void MusicMixerChange()
    {
        MusicMixer.SetFloat("MusicVolume", musicVolume);
    }

    public void ChoiceLevel(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }
}