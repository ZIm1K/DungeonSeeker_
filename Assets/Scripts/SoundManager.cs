using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class SoundSet
{
    public string volumeParameter;
    public Slider _slider;
}
public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public List<SoundSet> soundSetList;

    private const float _multiplier = 20f;

    private float _volumeValue;

    private void Start()
    {
        foreach (var soundSet in soundSetList) 
        {
            _volumeValue = PlayerPrefs.GetFloat(soundSet.volumeParameter, Mathf.Log10(soundSet._slider.value) * _multiplier);
            soundSet._slider.value = Mathf.Pow(10f, _volumeValue / _multiplier);
            audioMixer.SetFloat(soundSet.volumeParameter, _volumeValue);
        }          
    }

    public void OnChangeSliderValue(Slider slider) 
    {
        var volume = Mathf.Log10(slider.value) * _multiplier;
        foreach (var soundSet in soundSetList) 
        {
            if (soundSet._slider == slider) 
            {
                PlayerPrefs.SetFloat(soundSet.volumeParameter, volume);
                audioMixer.SetFloat(soundSet.volumeParameter, volume);
            }
        }
    }
}
