using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MonkeyEnemyAI : MonoBehaviour, EnemyAI
{
    private enum MonkeyAIState {
        TARGET,
        THROWING
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

    [Header("Monkey AI")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _rockPrefab;
    [SerializeField] private float _throwDamage;
    [SerializeField] private float _throwRadius;
    [SerializeField] private float _throwKnockback;
    [SerializeField] private float _throwStunTime;
    [SerializeField] private float _throwForce;
    [SerializeField] private float _rotSmoothness;
    [SerializeField] private Hittable _hittable;
    [SerializeField] private Sound _throwSound;
    private float _throwTimer;
    private float _stunTimer;
    private MonkeyAIState _aiState = MonkeyAIState.TARGET;
    private ScreenShaker _screenShaker;

    public void SetData(Transform __player, ScreenShaker __screenShaker) {
        _player = __player;
        _screenShaker = __screenShaker;
    }

    private void Start() {
        _seeker = GetComponent<Seeker>();
        _rb2D = GetComponent<Rigidbody2D>();
        
        InvokeRepeating("UpdatePathing", 0f, 0.5f);
        _throwTimer = 1f;
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
            case MonkeyAIState.TARGET:
                TargetAI();
                break;
            case MonkeyAIState.THROWING:
                _rb2D.velocity = Vector2.zero;
                break;
        }
    }

    private void TargetAI() {
        transform.right = Vector3.Lerp(transform.right, new Vector3(_player.position.x, _player.position.y, 0)-new Vector3(_rb2D.position.x, _rb2D.position.y, 0), Time.deltaTime*_rotSmoothness);

        _throwTimer = Mathf.Max(_throwTimer-Time.deltaTime, 0);

        if(Vector2.Distance(_player.position, _rb2D.position) <= _throwRadius && _throwTimer == 0) {
            _throwTimer = 0.5f;
            _animator.Play("Throw");
            _aiState = MonkeyAIState.THROWING;
        }

        if(_path == null) return;
        if(_currentWaypoint >= _path.vectorPath.Count) {
            UpdatePathing();
            return;
        }
        Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb2D.position).normalized;
        Vector2 velocity = direction*_speed;
        _rb2D.velocity = velocity;
        float distance = Vector2.Distance(_rb2D.position, _path.vectorPath[_currentWaypoint]);
        if(distance < _nextWayPointDistance) {
            _currentWaypoint++;
        }
    }

    public void AttackPlayer() {
        AudioManager.Instance.PlaySoundVaried(_throwSound);
        _screenShaker.ShakeRandom(0.5f, 25f, 0.1f);
        PhysicalEnemyDamageSource damageSource = Instantiate(_rockPrefab, _rb2D.position, Quaternion.identity).GetComponent<PhysicalEnemyDamageSource>();
        damageSource.SetData(_throwDamage, _throwKnockback, _throwStunTime, _hittable, 3f, true);
        damageSource.transform.position += transform.right*0.5f;
        damageSource.GetComponent<Rigidbody2D>().AddForce(transform.right*_throwForce, ForceMode2D.Impulse);
    }

    public void StopAttacking() {
        _throwTimer = 1f;
        _aiState = MonkeyAIState.TARGET;
    }

    public void Stun(float __duration) {
        _stunTimer = __duration;
        if(_aiState == MonkeyAIState.THROWING) _throwTimer = 1f;
        _aiState = MonkeyAIState.TARGET;
        _animator.Play("Idle");
    }
}
