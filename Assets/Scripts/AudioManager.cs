using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Music {
    public AudioClip Clip;
    [Range(0, 1)]
    public float Volume = 0.5f;
    [Range(-3, 3)]
    public float Pitch = 1f;
    public bool Loop;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameObject _soundPrefab;
    [SerializeField] private Music[] _musicSounds;
    private AudioSource[] _musicSources;
    private int _currentPlayingIndex;
    private float _soundVolume, _musicVolume;
    public static AudioManager Instance;

    private void Awake() {
        if(Instance) {
            Destroy(gameObject);
        } else {
            Instance = this;
            _currentPlayingIndex = -1;
            DontDestroyOnLoad(gameObject);
            LoadMusic();
        }
    }

    private void Start() {
        SetSoundVolume(PlayerPrefs.GetFloat("SoundVolume",  0.5f));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.5f));
    }

    public void SetSoundVolume(float __volume) {
        _soundVolume = __volume;
    }

    public void SetMusicVolume(float __volume) {
        _musicVolume = __volume;
        for(int i=0; i<_musicSounds.Length; i++) {
            _musicSources[i].volume = __volume;
            _musicSounds[i].Volume = __volume;
        }
    }

    private void LoadMusic() {
        List<AudioSource> addedMusic = new List<AudioSource>();
        foreach(Music music in _musicSounds) {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = music.Clip;
            source.pitch = music.Pitch;
            source.loop = music.Loop;
            source.playOnAwake = false;
            addedMusic.Add(source);
        }
        _musicSources = addedMusic.ToArray();
    }

    public void PlaySound(Sound __sound) {
        GameObject sound = Instantiate(_soundPrefab);
        AudioSource audioSource = sound.GetComponent<AudioSource>();
        audioSource.clip = __sound.Clip;
        audioSource.volume = __sound.Volume*_soundVolume;
        audioSource.pitch = __sound.Pitch;
        audioSource.Play();
        Destroy(sound, __sound.Clip.length+0.5f);
    }

    public void PlaySoundVaried(Sound __sound) {
        GameObject sound = Instantiate(_soundPrefab);
        AudioSource audioSource = sound.GetComponent<AudioSource>();
        audioSource.clip = __sound.Clip;
        audioSource.volume = __sound.Volume*_soundVolume;
        audioSource.pitch = __sound.Pitch+Random.Range(-0.1f, 0.1f);
        audioSource.Play();
        Destroy(sound, __sound.Clip.length+0.5f);
    }

    public void FadeToSong(int __musicIndex, float __fadeTime) {
        if(_currentPlayingIndex == __musicIndex) return;
        if(_currentPlayingIndex != -1) {
            StopAllCoroutines();
            StartCoroutine(FadeSongOut(_currentPlayingIndex, __fadeTime));
        }
        _musicSources[__musicIndex].volume = 0;
        _musicSources[__musicIndex].Play();
        StartCoroutine(FadeSongIn(__musicIndex, __fadeTime));
        _currentPlayingIndex = __musicIndex;
    }

    public void FadeOut(float __fadeTime) {
        if(_currentPlayingIndex != -1) {
            StopAllCoroutines();
            StartCoroutine(FadeSongOut(_currentPlayingIndex, __fadeTime));
            _currentPlayingIndex = -1;
        }
    }

    private IEnumerator FadeSongOut(int __musicIndex, float __fadeTime) {
        float remianing = __fadeTime;
        while(remianing > 0) {
            _musicSources[__musicIndex].volume -= _musicSounds[__musicIndex].Volume*(Time.deltaTime/__fadeTime);
            remianing -= Time.deltaTime;
            yield return null;
        }
        _musicSources[__musicIndex].Stop();
    }

    private IEnumerator FadeSongIn(int __musicIndex, float __fadeTime) {
        float remianing = __fadeTime;
        while(remianing > 0) {
            _musicSources[__musicIndex].volume = Mathf.Min(_musicSources[__musicIndex].volume+(_musicSounds[__musicIndex].Volume*(Time.deltaTime/__fadeTime)), _musicSounds[__musicIndex].Volume);
            remianing -= Time.deltaTime;
            yield return null;
        }
    }

    private void OnApplicationQuit() {
        PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
        PlayerPrefs.SetFloat("SoundVolume", _soundVolume);
    }
}
