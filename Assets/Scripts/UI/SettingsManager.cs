using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Slider _musicSlider;

    private void Start() {
        _soundSlider.value = PlayerPrefs.GetFloat("SoundVolume",  0.5f);
        _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume",  0.5f);
    }

    public void OnUpdateSoundSlider() {
        AudioManager.Instance.SetSoundVolume(_soundSlider.value);
    }

    public void OnUpdateMusicSlider() {
        AudioManager.Instance.SetMusicVolume(_musicSlider.value);
    }
}
