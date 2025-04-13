using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BearEnemyAI : MonoBehaviour, EnemyAI
{
    private enum BearAIState {
        TARGET,
        CLAWING
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

    [Header("Bear AI")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _clawDamage;
    [SerializeField] private float _clawRadius;
    [SerializeField] private float _clawKnockback;
    [SerializeField] private float _clawStunTime;
    [SerializeField] private float _rotSmoothness;
    [SerializeField] private Hittable _hittable;
    private float _clawTimer;
    private float _stunTimer;
    private BearAIState _aiState = BearAIState.TARGET;
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
            case BearAIState.TARGET:
                TargetAI();
                break;
            case BearAIState.CLAWING:
                _rb2D.velocity = Vector2.zero;
                break;
        }
    }

    private void TargetAI() {
        //transform.right = Vector3.Lerp(transform.right, new Vector3(_player.position.x, _player.position.y, 0)-new Vector3(_rb2D.position.x, _rb2D.position.y, 0), Time.deltaTime*_rotSmoothness);

        _clawTimer = Mathf.Max(_clawTimer-Time.deltaTime, 0);

        if(Vector2.Distance(_player.position, _rb2D.position) <= _clawRadius-0.15f && _clawTimer == 0) {
            _clawTimer = 1f;
            _animator.Play("Claw");
            _aiState = BearAIState.CLAWING;
        }

        if(_path == null) return;
        if(_currentWaypoint >= _path.vectorPath.Count) {
            UpdatePathing();
            return;
        }
        Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb2D.position).normalized;

        transform.right = Vector3.Lerp(transform.right, new Vector3(direction.x, direction.y, 0), Time.deltaTime*_rotSmoothness);

        Vector2 velocity = transform.right*_speed;
        _rb2D.velocity = velocity;
        float distance = Vector2.Distance(_rb2D.position, _path.vectorPath[_currentWaypoint]);
        if(distance < _nextWayPointDistance) {
            _currentWaypoint++;
        }
    }

    public void AttackPlayer() {
        if(Vector2.Distance(_player.position, _rb2D.position) <= _clawRadius) {
            _playerHealth.TakeDamage(_clawDamage, _hittable, false);
            _playerHealth.Knockback((Vector2)_player.position-_rb2D.position, _clawKnockback, _clawStunTime);
        }
    }

    public void StopClawing() {
        _clawTimer = 1f;
        _aiState = BearAIState.TARGET;
    }

    public void Stun(float __duration) {
        _stunTimer = __duration;
        if(_aiState == BearAIState.CLAWING) _clawTimer = 1f;
        _aiState = BearAIState.TARGET;
        _animator.Play("Idle");
    }
}
