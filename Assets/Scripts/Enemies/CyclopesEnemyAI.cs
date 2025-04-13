using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CyclopesEnemyAI : MonoBehaviour, EnemyAI
{
    private enum CyclopesAIState {
        TARGET,
        PUNCH,
        SLAM
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

    [Header("Cyclopes AI")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _punchDamage;
    [SerializeField] private float _punchRadius;
    [SerializeField] private float _punchKnockback;
    [SerializeField] private float _punchStunTime;
    [SerializeField] private float _slamDamage;
    [SerializeField] private float _slamRadius;
    [SerializeField] private float _slamKnockback;
    [SerializeField] private float _slamStunTime;
    [SerializeField] private float _rotSmoothness;
    [SerializeField] private Hittable _hittable;
    [SerializeField] private GameObject _particlesPrefab;
    [SerializeField] private Sound _slamSound;
    private float _punchTimer;
    private float _slamTimer;
    private float _stunTimer;
    private CyclopesAIState _aiState = CyclopesAIState.TARGET;
    private PlayerHealth _playerHealth;
    private ScreenShaker _screenShaker;

    public void SetData(Transform __player, ScreenShaker __screenShaker) {
        _player = __player;
        _playerHealth = __player.GetComponent<PlayerHealth>();
        _screenShaker = __screenShaker;
    }

    private void Start() {
        if(_player) {
            _playerHealth = _player.GetComponent<PlayerHealth>();
        }
        _seeker = GetComponent<Seeker>();
        _rb2D = GetComponent<Rigidbody2D>();
        
        InvokeRepeating("UpdatePathing", 0f, 0.5f);
        _slamTimer = 2f;
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
            case CyclopesAIState.TARGET:
                TargetAI();
                break;
            case CyclopesAIState.PUNCH:
                _rb2D.velocity = Vector2.zero;
                break;
            case CyclopesAIState.SLAM:
                _rb2D.velocity = Vector2.zero;
                break;
        }
    }

    private void TargetAI() {
        //transform.right = Vector3.Lerp(transform.right, new Vector3(_player.position.x, _player.position.y, 0)-new Vector3(_rb2D.position.x, _rb2D.position.y, 0), Time.deltaTime*_rotSmoothness);

        _punchTimer = Mathf.Max(_punchTimer-Time.deltaTime, 0);
        _slamTimer = Mathf.Max(_slamTimer-Time.deltaTime, 0);

        if(Vector2.Distance(_player.position, _rb2D.position) <= _punchRadius-0.15f && _punchTimer == 0) {
            _punchTimer = 1f;
            _animator.Play("Punch");
            _aiState = CyclopesAIState.PUNCH;
        }

        if(Vector2.Distance(_player.position, _rb2D.position) <= _slamRadius-0.15f && _slamTimer == 0) {
            _slamTimer = 5f;
            _animator.Play("Slam");
            _aiState = CyclopesAIState.SLAM;
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
        if(Vector2.Distance(_player.position, _rb2D.position) <= _punchRadius) {
            _playerHealth.TakeDamage(_punchDamage, _hittable, false);
            _playerHealth.Knockback((Vector2)_player.position-_rb2D.position, _punchKnockback, _punchStunTime);
        }
    }

    public void StopPunching() {
        _punchTimer = 1f;
        _aiState = CyclopesAIState.TARGET;
    }

    public void StopSlamming() {
        _slamTimer = 4f+Random.Range(-1f, 0.25f);
        _aiState = CyclopesAIState.TARGET;
    }

    public void Stun(float __duration) {
        _stunTimer = __duration;
        if(_aiState == CyclopesAIState.PUNCH) _punchTimer = 1f;
        if(_aiState == CyclopesAIState.SLAM) _slamTimer = 4f;
        _aiState = CyclopesAIState.TARGET;
        _animator.Play("Idle");
    }

    public void Slam() {
        AudioManager.Instance.PlaySoundVaried(_slamSound);
        _screenShaker.ShakeRandom(1f, 25f, 0.1f);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _slamRadius);
        GameObject particles = Instantiate(_particlesPrefab, transform.position, Quaternion.identity);
        Destroy(particles, 5f);
        foreach(Collider2D col in colliders) {
            PlayerHealth playerHealth = col.GetComponent<PlayerHealth>();
            if(playerHealth) {
                playerHealth.TakeDamage(_slamDamage, _hittable, false);
                playerHealth.Knockback((Vector2)_player.position-_rb2D.position, _slamKnockback, _slamStunTime);
            }
        }

    }
}
