using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pauser : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private PlayerMovement _playerMovement;
    private bool _paused;

    private void Start() {
        _paused = false;
        _pausePanel.SetActive(_paused);
        if(_paused) {
            _playerMovement.SetLooking(false);
            Time.timeScale = 0;
        } else {
            _playerMovement.SetLooking(true);
            Time.timeScale = 1;
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            _paused = !_paused;
            _pausePanel.SetActive(_paused);
            if(_paused) {
                _playerMovement.SetLooking(false);
                Time.timeScale = 0;
            } else {
                _playerMovement.SetLooking(true);
                Time.timeScale = 1;
            }
        }
    }
}
