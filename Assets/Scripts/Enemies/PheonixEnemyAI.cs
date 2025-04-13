using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PheonixEnemyAI : MonoBehaviour, EnemyAI
{
    private enum PheonixAIState {
        TARGET,
        FLAME_BURST
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

    [Header("Pheonix AI")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _flameBurstPrefab;
    [SerializeField] private float _flameDamage;
    [SerializeField] private float _flameRadius;
    [SerializeField] private float _flameKnockback;
    [SerializeField] private float _flameStunTime;
    [SerializeField] private float _flameForce;
    [SerializeField] private float _rotSmoothness;
    [SerializeField] private Hittable _hittable;
    [SerializeField] private Sound _flameBurstSound;
    private float _flameTimer;
    private float _stunTimer;
    private PheonixAIState _aiState = PheonixAIState.TARGET;
    private ScreenShaker _screenShaker;

    public void SetData(Transform __player, ScreenShaker __screenShaker) {
        _player = __player;
        _screenShaker = __screenShaker;
    }

    private void Start() {
        _seeker = GetComponent<Seeker>();
        _rb2D = GetComponent<Rigidbody2D>();
        
        InvokeRepeating("UpdatePathing", 0f, 0.5f);
        _flameTimer = 1f;
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
            case PheonixAIState.TARGET:
                TargetAI();
                break;
            case PheonixAIState.FLAME_BURST:
                _rb2D.velocity = Vector2.zero;
                break;
        }
    }

    private void TargetAI() {
        transform.right = Vector3.Lerp(transform.right, new Vector3(_player.position.x, _player.position.y, 0)-new Vector3(_rb2D.position.x, _rb2D.position.y, 0), Time.deltaTime*_rotSmoothness);

        _flameTimer = Mathf.Max(_flameTimer-Time.deltaTime, 0);

        if(Vector2.Distance(_player.position, _rb2D.position) <= _flameRadius && _flameTimer == 0) {
            _flameTimer = 0.5f;
            _animator.Play("Flame Burst");
            _aiState = PheonixAIState.FLAME_BURST;
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
        AudioManager.Instance.PlaySoundVaried(_flameBurstSound);
        _screenShaker.ShakeRandom(0.5f, 25f, 0.1f);
        for(int i=0; i<3; i++) {
            PhysicalEnemyDamageSource damageSource = Instantiate(_flameBurstPrefab, _rb2D.position, Quaternion.identity).GetComponent<PhysicalEnemyDamageSource>();
            damageSource.transform.right = transform.right;
            damageSource.transform.Rotate(0,0, -15f+(15f*i));
            damageSource.SetData(_flameDamage, _flameKnockback, _flameStunTime, _hittable, 3f, true);
            damageSource.transform.position += damageSource.transform.right*0.5f;
            damageSource.GetComponent<Rigidbody2D>().AddForce(damageSource.transform.right*_flameForce, ForceMode2D.Impulse);
        }
    }

    public void StopAttacking() {
        if(_aiState == PheonixAIState.FLAME_BURST) _flameTimer = 1.25f+Random.Range(-0.75f, 0.25f);
        _aiState = PheonixAIState.TARGET;
    }

    public void Stun(float __duration) {
        _stunTimer = __duration;
        _flameTimer = 1.25f;
        _aiState = PheonixAIState.TARGET;
        _animator.Play("Idle");
    }
}
