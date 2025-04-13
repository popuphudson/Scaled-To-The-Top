using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Sound _takeDamageSound;
    [SerializeField] private Sound _deathSound;
    [SerializeField] private Animator _deathAnimator;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private BasicPlayerAttacks _playerAttacks;
    [SerializeField] private PopUpTextCreator _popUpTextCreator;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _healthRegenPerSecond;
    private ScreenShaker _screenShaker;
    private float _health;
    private float _healTimer;
    private bool _invulnerable;
    private float _hitGrace = 0;

    private void Start() {
        _health = _maxHealth;
        _invulnerable = false;
        _screenShaker = _playerStats.GetScreenShaker();
    }

    private void Update() {
        _healTimer = Mathf.Max(_healTimer-Time.deltaTime, 0);
        _hitGrace = Mathf.Max(_hitGrace-Time.deltaTime, 0);
        if(_healTimer == 0) {
            Heal(_healthRegenPerSecond*Time.deltaTime, true);
        }
    }

    public void TakeDamage(float __damage, Hittable __from, bool __ignoreHitGrace) {
        if(_invulnerable) return;
        if(_hitGrace > 0 && !__ignoreHitGrace) return;
        AudioManager.Instance.PlaySoundVaried(_takeDamageSound);
        _hitGrace = 0.5f;
        float inflictedDamage = __damage;
        _playerStats.OnPlayerDamaged(ref inflictedDamage);
        _playerStats.OnPlayerHit(__from);
        _health -= inflictedDamage;
        _healTimer = 1f;
        _popUpTextCreator.SpawnPlayerPopUpText($"<color=#ffff55ff>{inflictedDamage}</color>", transform.position);
        if(_health <= 0) {
            AudioManager.Instance.PlaySoundVaried(_deathSound);
            _playerStats.OnPlayerDeath();
            _deathAnimator.Play("Death");
            _playerMovement.StopInput();
            _playerAttacks.StopInput();
            gameObject.SetActive(false);
        }
    }

    public void Knockback(Vector2 __dir, float __power, float __stunTime) {
        float inflictedPower = __power;
        _playerStats.OnPlayerKnockedback(ref inflictedPower);
        if(__dir == Vector2.zero) _screenShaker.ShakeRandom(__power/2f, 50f, 0.1f);
        else _screenShaker.ShakeDirectionOnce(__dir, __power/2f, 25f, 0.5f);
        _playerMovement.Knockback(__dir, inflictedPower, __stunTime);
    }

    public float GetHealth() {
        return _health;
    }

    public float GetMaxHealth() {
        return _maxHealth;
    }

    public void Heal(float __health, bool __regened) {
        float healApplied = __health;
        _playerStats.OnPlayerHealed(ref healApplied);
        _health += healApplied;
        if(!__regened) _popUpTextCreator.SpawnPlayerPopUpText($"<color=#88ff88ff>{healApplied}</color>", transform.position);
        if(_health >= _maxHealth) {
            _health = _maxHealth;
        }
    }

    public void SetInvulnerable(bool __invulnerable) {
        _invulnerable = __invulnerable;
    }
}
