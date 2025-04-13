using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniverseBossAI : MonoBehaviour, EnemyAI
{
    [SerializeField] private PopUpTextCreator _popUpText;
    [SerializeField] private Animator _animator;
    [SerializeField] private Hittable _hittable;
    [SerializeField] private Transform _target;
    [SerializeField] private float _contactDamage;
    [SerializeField] private float _contactKnockback;
    [SerializeField] private float _bulletDamage;
    [SerializeField] private float _bulletKnockback;
    [SerializeField] private float _bulletForce;
    [SerializeField] private Vector3 _meteorPos;
    [SerializeField] private float _meteorDamage;
    [SerializeField] private float _meteorKnockback;
    [SerializeField] private float _meteorForce;
    [SerializeField] private GameObject _bulletPrefab, _meteorPrefab;
    [SerializeField] private GameObject[] _spawnableEnemies;
    [SerializeField] private Vector3[] _spawnLocations;
    [SerializeField] private Transform _spawnHolder;
    [SerializeField] private GameObject _blackHoleEffect;
    [SerializeField] private GameObject _deathEffect;
    [SerializeField] private Sound _blackHoleSound;
    [SerializeField] private Sound _spawnMeteorSound, _spawnGalaxySound;
    [SerializeField] private Sound _dashSound;
    [SerializeField] private Sound _spawnEnemiesSound;
    [SerializeField] private Sound _deathSound;
    [SerializeField] private Image _healthBarImage;
    private PlayerHealth _playerHealth;
    private PlayerMovement _playerMovement;
    private ScreenShaker _screenShaker;
    private bool _contactDamageEnabled;
    private bool _startedBlackhole;
    private float _blackHolePull;
    private float _bulletShift;

    private void Start() {
        AudioManager.Instance.FadeToSong(2, 1.5f);
        _playerHealth = _target.GetComponent<PlayerHealth>();
        _playerMovement = _target.GetComponent<PlayerMovement>();
        _screenShaker = _target.GetComponent<PlayerStats>().GetScreenShaker();
        _contactDamageEnabled = false;
    }

    public void SpawnBullets() {
        AudioManager.Instance.PlaySoundVaried(_spawnGalaxySound);
        _screenShaker.ShakeRandom(1f, 50f, 0.1f);
        for(int i=0; i<18; i++) {
            PhysicalEnemyDamageSource bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity).GetComponent<PhysicalEnemyDamageSource>();
            bullet.SetData(_bulletDamage, _bulletKnockback, 0.1f, _hittable, 10f, true);
            bullet.transform.Rotate(0, 0, (i*(360/18))+_bulletShift);
            bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.right*_bulletForce, ForceMode2D.Impulse);
        }
        _bulletShift += 15;
    }

    public void SpawnMeteors() {
        AudioManager.Instance.PlaySoundVaried(_spawnMeteorSound);
        _screenShaker.ShakeRandom(2f, 50f, 0.2f);
        for(int i=0; i<9; i++) {
            PhysicalEnemyDamageSource meteor = Instantiate(_meteorPrefab, _meteorPos, Quaternion.identity).GetComponent<PhysicalEnemyDamageSource>();
            meteor.SetData(_meteorDamage, _meteorKnockback, 0.1f, _hittable, 15f, true);
            meteor.transform.Rotate(0, 0, 190+(((i*10)+_bulletShift)%90));
            meteor.GetComponent<Rigidbody2D>().AddForce(meteor.transform.right*_meteorForce, ForceMode2D.Impulse);
        }
        _bulletShift += 5f;
    }

    public void StartBlackHole() {
        AudioManager.Instance.PlaySoundVaried(_blackHoleSound);
        _screenShaker.ShakeRandom(2f, 25f, 5f);
        _startedBlackhole = true;
        _blackHolePull = 1f;
        EnableContactDamage();
        foreach(Transform enemy in _spawnHolder) {
            enemy.GetComponent<Hittable>().TakeDamage(1000, null);
            _hittable.Heal(100);
        }
        GameObject temp = Instantiate(_blackHoleEffect, transform.position, Quaternion.identity);
        Destroy(temp, 10f);
    }

    public void StopBlackHole() {
        AudioManager.Instance.PlaySoundVaried(_blackHoleSound);
        DisableContactDamage();
        _playerMovement.StopPulling();
        _startedBlackhole = false;
        _blackHolePull = 1f;
    }

    public void EnableContactDamage() {
        _contactDamageEnabled = true;
    }

    public void DisableContactDamage() {
        _contactDamageEnabled = false;
    }

    public void PlayDashSound() {
        AudioManager.Instance.PlaySoundVaried(_dashSound);
    }

    public void SpawnEnemies() {
        AudioManager.Instance.PlaySoundVaried(_spawnEnemiesSound);
        for(int i=0; i<_spawnLocations.Length; i++) {
            Hittable hittable = Instantiate(_spawnableEnemies[Random.Range(0, _spawnableEnemies.Length)], _spawnHolder).GetComponent<Hittable>();
            hittable.transform.position = _spawnLocations[i];
            hittable.SetData(_popUpText);
            hittable.GetComponent<EnemyAI>().SetData(_target, _screenShaker);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.transform.GetComponent<PlayerHealth>() && _contactDamageEnabled) {
            _playerHealth.TakeDamage(_contactDamage, _hittable, false);
            _playerHealth.Knockback((_target.position-transform.position).normalized, _contactKnockback, 0.1f);
        }
    }

    public void SetData(Transform __player, ScreenShaker __screenShaker) {
        return;
    }

    public void Stun(float __duration) {
        return;
    }

    public void DecideNextP1Action() {
        if(Random.Range(0, 100) < 50 || _spawnHolder.childCount > 16) {
            int chosen = Random.Range(0, 3);
            if(chosen == 1) {
                chosen = 3;
            }
            _animator.SetInteger("Action", chosen);
        }
        else _animator.SetInteger("Action", 1);
    }

    public void DecideNextP2Action() {
        if(Random.Range(0, 100) < 70 || _spawnHolder.childCount > 20) {
            int chosen = Random.Range(0, 5);
            if(chosen == 1) {
                chosen = 5;
            }
            _animator.SetInteger("Action", chosen);
        }
        else _animator.SetInteger("Action", 1);
    }

    private void Update() {
        if(_hittable.GetHealth() < _hittable.GetMaxHealth()/2f && !_animator.GetBool("Phase2")) {
            _animator.SetBool("Phase2", true);
            _hittable.SetDefensePercentage(0.5f);
            _healthBarImage.color = new Color(1f, 0.5f, 0);
        }
        if(_startedBlackhole) {
            _blackHolePull += Time.deltaTime;
            _playerMovement.PullTowards(transform.position, _blackHolePull/4.75f);
        }
    }

    public void StartDeath() {
        AudioManager.Instance.PlaySound(_deathSound);
        _screenShaker.ShakeRandom(1f, 25f, 3f);
        AudioManager.Instance.FadeOut(1f);
    }

    public void Dead() {
        _screenShaker.ShakeRandom(10f, 100f, 0.5f);
        GameObject temp = Instantiate(_deathEffect, transform.position, Quaternion.identity);
        Destroy(temp, 10f);
        _playerMovement.GetComponent<PlayerXP>().AddXP(1);
        foreach(Transform enemy in _spawnHolder) {
            enemy.GetComponent<Hittable>().TakeDamage(1000, null);
        }
        Destroy(gameObject);
    }
}
