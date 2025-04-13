using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Sound _selectSound;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _howToPlay;
    [SerializeField] private GameObject _settings;

    private void Start() {
        AudioManager.Instance.FadeToSong(0, 1);
    }

    public void Quit() {
        Application.Quit();
    }

    public void GoToSettings() {
        AudioManager.Instance.PlaySoundVaried(_selectSound);
        _settings.SetActive(true);
        _mainMenu.SetActive(false);
    }

    public void StartGame() {
        AudioManager.Instance.PlaySoundVaried(_selectSound);
        _startMenu.SetActive(true);
        _mainMenu.SetActive(false);
    }

    public void HowToPlay() {
        AudioManager.Instance.PlaySoundVaried(_selectSound);
        _howToPlay.SetActive(true);
        _mainMenu.SetActive(false);
    }

    public void BackToMenu() {
        AudioManager.Instance.PlaySoundVaried(_selectSound);
        _settings.SetActive(false);
        _howToPlay.SetActive(false);
        _startMenu.SetActive(false);
        _mainMenu.SetActive(true);
    }

    public void GoIntoGame() {
        AudioManager.Instance.PlaySoundVaried(_selectSound);
        _animator.Play("MainMenuIntoGame");
    }

    public void LoadStage1Scene() {
        SceneManager.LoadScene(1);
    }
}
