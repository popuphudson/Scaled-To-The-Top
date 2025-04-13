using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerDisplay : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private BasicPlayerAttacks _basicPlayerAttacks;
    [SerializeField] private PlayerXP _playerXP;
    [SerializeField] private PlayerMovement _playerMovement;
    [Header("UI")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _primaryCoolDownSlider, _secondaryCoolDownSlider, _specialCoolDownSlider, _dashCoolDownSlider;
    [SerializeField] private Slider _xpSlider;
    [SerializeField] private GameObject _upgradeText;
    [Header("Next Level")]
    [SerializeField] private int _nextLevelIndex; 
    [SerializeField] private Hittable _theUniverseItself;
    private void Start() {
        _healthSlider.maxValue = _playerHealth.GetMaxHealth();
        _primaryCoolDownSlider.maxValue = _basicPlayerAttacks.GetPrimaryCoolDown();
        _secondaryCoolDownSlider.maxValue = _basicPlayerAttacks.GetSecondaryCoolDown();
        _specialCoolDownSlider.maxValue = _basicPlayerAttacks.GetSpecialCoolDown();
        _dashCoolDownSlider.maxValue = _playerMovement.GetDashCoolDown();
        if(_nextLevelIndex != 5) _xpSlider.maxValue = _playerXP.GetXPToNextSize();
        else _xpSlider.maxValue = _theUniverseItself.GetMaxHealth();
    }

    private void Update() {
        _healthSlider.value = _playerHealth.GetHealth();
        _primaryCoolDownSlider.value = _basicPlayerAttacks.GetPrimaryCoolDownTimer();
        _secondaryCoolDownSlider.value = _basicPlayerAttacks.GetSecondaryCoolDownTimer();
        _specialCoolDownSlider.value = _basicPlayerAttacks.GetSpecialCoolDownTimer();
        _dashCoolDownSlider.value = _playerMovement.GetDashCoolDownTimer();
        if(_nextLevelIndex != 5) _upgradeText.SetActive(_xpSlider.value>=_xpSlider.maxValue);
        else _upgradeText.SetActive(_playerXP.GetXP()>0);
        if(_nextLevelIndex != 5) _xpSlider.value = _playerXP.GetXP();
        else _xpSlider.value = _theUniverseItself.GetHealth();
    }

    public void ReturnToMenu() {
        SceneManager.LoadScene(0);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void NextLevel() {
        SceneManager.LoadScene(_nextLevelIndex);
    }
}
