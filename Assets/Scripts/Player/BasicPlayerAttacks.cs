using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AttackType {
    PRIMARY,
    SECONDARY,
    SPECIAL
}

public class BasicPlayerAttacks : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerXP _playerXP;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerStats _playerStats; 
    [SerializeField] private ScreenShaker _screenShaker; 

    [Header("Abilities")]
    [SerializeField] private float _primaryDamage;
    [SerializeField] private float _primaryCoolDown;
    [SerializeField] private float _primaryKnockback;
    [SerializeField] private float _primaryStunTime;
    [SerializeField] private float _secondaryDamage;
    [SerializeField] private float _secondaryCoolDown;
    [SerializeField] private float _secondaryKnockback;
    [SerializeField] private float _secondaryStunTime;
    [SerializeField] private float _specialDamage;
    [SerializeField] private float _specialCoolDown;
    [SerializeField] private float _specialKnockback;
    [SerializeField] private float _specialStunTime;
    [SerializeField] private Sound _laserBeamSound; 
    private float _primaryCoolDownTimer, _secondaryCoolDownTimer, _specialCoolDownTimer;
    private bool _stopInput = false;
    private bool _beaming = false;

    private void Update() {
        UpdateCoolDownTimers();
        ProcessInputs();
    }

    public void StopInput() {
        _stopInput = true;
    }

    private void ProcessInputs() {
        if(_stopInput) return;
        if(_beaming) {
            AudioManager.Instance.PlaySoundVaried(_laserBeamSound);
            return;
        }
        if(Input.GetMouseButtonDown(0) && _primaryCoolDownTimer == 0) {
            _primaryCoolDownTimer = _primaryCoolDown;
            _playerStats.OnPlayerPrimaryAttack();
            _animator.Play("Punch");
        }
        if(Input.GetMouseButtonDown(1) && _secondaryCoolDownTimer == 0) {
            _secondaryCoolDownTimer = _secondaryCoolDown;
            _playerStats.OnPlayerSecondaryAttack();
            _animator.Play("Kick");
        }
        if(Input.GetKeyDown(KeyCode.E) && _specialCoolDownTimer == 0) {
            _specialCoolDownTimer = _specialCoolDown;
            _playerStats.OnPlayerSpecialAttack();
            _animator.Play("TearBeam");
            _playerMovement.SetLookEfficient(3f);
            _beaming = true;
        }
    }

    public void EndBeam() {
        _beaming = false;
        _playerMovement.SetLookEfficient(-1);
    }

    public void DoDamageWithPrimary(Hittable __hit) {
        float damageDelt = _primaryDamage;
        _playerStats.OnInflictDamage(ref damageDelt);
        _playerStats.OnEnemyHit(__hit);
        __hit.TakeDamage(damageDelt, _playerXP);
        __hit.Knockback(DamageDirection(__hit), _primaryKnockback, _primaryStunTime);
        _screenShaker.ShakeDirectionOnce(transform.right, 0.5f, 10f, 0.25f);
    }
    public void DoDamageWithSecondary(Hittable __hit) {
        float damageDelt = _secondaryDamage;
        _playerStats.OnInflictDamage(ref damageDelt);
        _playerStats.OnEnemyHit(__hit);
        __hit.TakeDamage(damageDelt, _playerXP);
        __hit.Knockback(DamageDirection(__hit), _secondaryKnockback, _secondaryStunTime);
        _screenShaker.ShakeDirectionOnce(transform.right, 1f, 12f, 0.5f);
    }
    public void DoDamageWithSpecial(Hittable __hit) {
        float damageDelt = _specialDamage;
        _playerStats.OnInflictDamage(ref damageDelt);
        _playerStats.OnEnemyHit(__hit);
        __hit.TakeDamage(damageDelt, _playerXP);
        __hit.Knockback(DamageDirection(__hit), _specialKnockback, _specialStunTime);
        _screenShaker.ShakeRandom(1f, 25f, 0.25f);
    }

    private Vector2 DamageDirection(Hittable __hit) {
        return (Vector2)(__hit.transform.position-transform.position).normalized;
    }

    private void UpdateCoolDownTimers() {
        _primaryCoolDownTimer = Mathf.Max(_primaryCoolDownTimer-Time.deltaTime, 0);
        _secondaryCoolDownTimer = Mathf.Max(_secondaryCoolDownTimer-Time.deltaTime, 0);
        _specialCoolDownTimer = Mathf.Max(_specialCoolDownTimer-Time.deltaTime, 0);
    }

    public float GetPrimaryCoolDown() {
        return _primaryCoolDown;
    }
    public float GetPrimaryCoolDownTimer() {
        return _primaryCoolDownTimer;
    }
    public float GetSecondaryCoolDown() {
        return _secondaryCoolDown;
    }
    public float GetSecondaryCoolDownTimer() {
        return _secondaryCoolDownTimer;
    }
    public float GetSpecialCoolDown() {
        return _specialCoolDown;
    }
    public float GetSpecialCoolDownTimer() {
        return _specialCoolDownTimer;
    }
}
