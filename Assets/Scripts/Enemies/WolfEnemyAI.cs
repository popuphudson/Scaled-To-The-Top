using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Pathfinding;

public class WolfEnemyAI : MonoBehaviour, EnemyAI
{
    private enum WolfAIState {
        CHARGING,
        TARGET,
        BITING
    }

    [Header("A* Pathfinding and Movement")]
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _nextWayPointDistance = 3f;
    [SerializeField] private Transform _player;
    [SerializeField] private LayerMask _obsticalLayer;
    private Path _path;
    private int _currentWaypoint = 0;
    private Seeker _seeker;
    private Rigidbody2D _rb2D;

    [Header("Wolf AI")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _biteDamage;
    [SerializeField] private float _biteRadius;
    [SerializeField] private float _biteKnockback;
    [SerializeField] private float _biteStunTime;
    [SerializeField] private float _chargeDamage;
    [SerializeField] private float _chargeRadius;
    [SerializeField] private float _chargeKnockback;
    [SerializeField] private float _chargeStunTime;
    [SerializeField] private float _chargeForce;
    [SerializeField] private float _chargeCoolDown;
    [SerializeField] private WolfCharge _chargePhysicalDamager;
    [SerializeField] private float _rotSmoothness;
    [SerializeField] private Hittable _hittable;
    private float _chargeCoolDownTimer;
    private float _chargeTimer;
    private float _biteTimer;
    private float _stunTimer;
    private Vector2 _chargeDir;
    private WolfAIState _aiState = WolfAIState.TARGET;
    private PlayerHealth _playerHealth;

    public void SetData(Transform __player, ScreenShaker __screenShaker) {
        _player = __player;
        _playerHealth = __player.GetComponent<PlayerHealth>();
    }

    private void Start() {
        if(_player) {
            _playerHealth = _player.GetComponent<PlayerHealth>();
        }
        _seeker = GetComponent<Seeker>();
        _rb2D = GetComponent<Rigidbody2D>();
        
        InvokeRepeating("UpdatePathing", 0f, 0.5f);
        _chargePhysicalDamager.SetData(_chargeDamage, _chargeKnockback, _chargeStunTime, this);
        _chargeTimer = _chargeCoolDown;
        _biteTimer = 1f;
    }

    private void UpdatePathing() {
        if(!_seeker.IsDone()) return;
        _seeker.StartPath(_rb2D.position, _player.position, OnPathComplete);
    }

    private void OnPathComplete(Path __p) {
        if(!__p.error) {
            _path = __p;
            _currentWaypoint = 0;
        }
    }


    void Update() {
        if(_stunTimer > 0) {
            _stunTimer = Mathf.Max(_stunTimer-Time.deltaTime, 0);
            return;
        }
        _rb2D.bodyType = RigidbodyType2D.Kinematic;
        switch(_aiState) {
            case WolfAIState.TARGET:
                TargetAI();
                break;
            case WolfAIState.CHARGING:
                ChargeAI();
                break;
            case WolfAIState.BITING:
                _rb2D.velocity = Vector2.zero;
                break;
        }
    }

    private void TargetAI() {
        //transform.right = Vector3.Lerp(transform.right, new Vector3(_player.position.x, _player.position.y, 0)-new Vector3(_rb2D.position.x, _rb2D.position.y, 0), Time.deltaTime*_rotSmoothness);
        
        _chargePhysicalDamager.gameObject.SetActive(false);
        _chargeCoolDownTimer = Mathf.Max(_chargeCoolDownTimer-Time.deltaTime, 0);
        if(!Physics2D.Linecast(_rb2D.position, _player.position, _obsticalLayer) && Vector2.Distance(_player.position, _rb2D.position) <= _chargeRadius && _chargeCoolDownTimer == 0) {
            _chargeCoolDownTimer = 1f+Random.Range(-0.25f, 0.25f);
            _aiState = WolfAIState.CHARGING;
            _chargeTimer = 0.3f+Random.Range(-0.25f, 0.25f);;
            _biteTimer = 0f;
            _chargeDir = ((Vector2)_player.position - _rb2D.position).normalized;
        }

        _biteTimer = Mathf.Max(_biteTimer-Time.deltaTime, 0);

        if(Vector2.Distance(_player.position, _rb2D.position) <= _biteRadius-0.15f && _biteTimer == 0) {
            _biteTimer = 1f;
            _animator.Play("Bite");
            _aiState = WolfAIState.BITING;
        }

        if(_path == null) return;
        if(_currentWaypoint >= _path.vectorPath.Count) {
            UpdatePathing();
            return;
        }
        Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb2D.position).normalized;

        transform.right = Vector3.Lerp(transform.right, new Vector3(direction.x, direction.y, 0), Time.deltaTime*_rotSmoothness);

        Vector2 velocity = direction*_speed;
        _rb2D.velocity = velocity;
        float distance = Vector2.Distance(_rb2D.position, _path.vectorPath[_currentWaypoint]);
        if(distance < _nextWayPointDistance) {
            _currentWaypoint++;
        }
    }

    private void ChargeAI() {
        transform.right = Vector3.Lerp(transform.right, new Vector3(_chargeDir.x, _chargeDir.y, 0), Time.deltaTime*_rotSmoothness);
        _chargeCoolDownTimer = Mathf.Max(_chargeCoolDownTimer-Time.deltaTime, 0);
        _chargePhysicalDamager.gameObject.SetActive(false);
        if(_chargeCoolDownTimer > 0) { 
            _rb2D.velocity = Vector2.zero;
            return;
        }

        _rb2D.velocity = _chargeDir*_chargeForce;
        _chargeTimer = Mathf.Max(_chargeTimer-Time.deltaTime, 0);
        _chargePhysicalDamager.gameObject.SetActive(true);

        if(_chargeTimer == 0) {
            _chargeCoolDownTimer = _chargeCoolDown;
            _aiState = WolfAIState.TARGET;
        }
    }

    public void AttackPlayer() {
        if(Vector2.Distance(_player.position, _rb2D.position) <= _biteRadius) {
            _playerHealth.TakeDamage(_biteDamage, _hittable, false);
            _playerHealth.Knockback((Vector2)_player.position-_rb2D.position, _biteKnockback, _biteStunTime);
        }
    }

    public void StopBiting() {
        _biteTimer = 1f+Random.Range(-0.25f, 0.25f);;
        _aiState = WolfAIState.TARGET;
    }

    public void Stun(float __duration) {
        _stunTimer = __duration;
        _animator.Play("Idle");
        if(_aiState == WolfAIState.BITING) _biteTimer = 1f+Random.Range(-0.25f, 0.25f);
        _aiState = WolfAIState.TARGET;
    }

    public void StopCharge() {
        _aiState = WolfAIState.TARGET;
        _animator.Play("Idle");
    }
}
