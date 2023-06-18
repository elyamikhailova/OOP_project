using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;


public class AudioMixerController : MonoBehaviour
{
    public AudioMixer masterMixer;

    public AnimationCurve volumeCurve;

    public Slider musicSlider;
    public Slider sfxSlider;

    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;

    public void SetMusicVolume()
    {
        masterMixer.SetFloat("MusicVolume", -80f + (80f * (volumeCurve.Evaluate( musicSlider.value))));
        musicText.text = $"Музыка: {(Mathf.RoundToInt(musicSlider.value * 100f))}%";
    }

    public void SetSFXolume()
    {
        masterMixer.SetFloat("SFXVolume", -80f + (80f * (volumeCurve.Evaluate(sfxSlider.value))));
        sfxText.text = $"SFX громкость: {(Mathf.RoundToInt(sfxSlider.value * 100f))}%";
    }

    public void SaveVolumes()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }

    public void LoadVolumes()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        if (PlayerPrefs.HasKey("SFXVolume"))
            sfxSlider.value = PlayerPrefs.GetFloat("SfXVolume");
    }

    private void OnApplicationQuit()
    {
        SaveVolumes();
    }

    private void Start()
    {
        LoadVolumes();
    }
}
