using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    [SerializeField] private Animator _deathAnimator;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private float _xpRequiredForNextSize;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private BasicPlayerAttacks _playerAttacks;
    [SerializeField] private Sound _matterGainSound;
    [SerializeField] private Sound _growSound;
    private float _xp;
    private bool _canUpgrade = false;

    public void AddXP(float __addedXP) {
        _playerMovement.KilledEnemy();
        AudioManager.Instance.PlaySoundVaried(_matterGainSound);
        _xp += __addedXP;
        if(_xp >= _xpRequiredForNextSize) {
            _canUpgrade = true;
        }
    }

    public float GetXP() {
        return _xp;
    }

    public float GetXPToNextSize() {
        return _xpRequiredForNextSize;
    }

    private void Update() {
        if(_canUpgrade) {
            if(Input.GetKeyDown(KeyCode.Space)) {
                _canUpgrade = false;
                AudioManager.Instance.PlaySoundVaried(_growSound);
                _deathAnimator.Play("LevelExit");
                _playerAnimator.Play("Grow");
                _playerAttacks.StopInput();
                _playerMovement.StopInput();
                _playerHealth.SetInvulnerable(true);
            }
        }
    }
}
