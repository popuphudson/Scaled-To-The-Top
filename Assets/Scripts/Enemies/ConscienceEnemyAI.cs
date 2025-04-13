using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ConscienceEnemyAI : MonoBehaviour, EnemyAI
{
    private enum ConscienceAIState {
        TARGET,
        THOUGHT_OVERLOAD,
        THOUGHT_EXPLOSION
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

    [Header("Conscience AI")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _thoughtExplosionDamage;
    [SerializeField] private float _thoughtExplosionRadius;
    [SerializeField] private float _thoughtExplosionKnockback;
    [SerializeField] private float _thoughtExplosionStunTime;
    [SerializeField] private float _thoughtStunRadius;
    [SerializeField] private float _thoughtStunStunTime;
    [SerializeField] private ParticleSystem _thoughtStunParticles;
    [SerializeField] private GameObject _thoughtExplosionParticles;
    [SerializeField] private float _rotSmoothness;
    [SerializeField] private Hittable _hittable;
    [SerializeField] private Sound _explosionSound, _stunSound;
    private float _thoughtStunCoolDownTimer;
    private float _thoughtExplosionTimer;
    private float _stunTimer;
    private ConscienceAIState _aiState = ConscienceAIState.TARGET;
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
        _thoughtStunCoolDownTimer = 5f;
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
            case ConscienceAIState.TARGET:
                TargetAI();
                break;
            case ConscienceAIState.THOUGHT_EXPLOSION:
                _rb2D.velocity = Vector2.zero;
                break;
            case ConscienceAIState.THOUGHT_OVERLOAD:
                _rb2D.velocity = Vector2.zero;
                break;
        }
    }

    private void TargetAI() {
        //transform.right = Vector3.Lerp(transform.right, new Vector3(_player.position.x, _player.position.y, 0)-new Vector3(_rb2D.position.x, _rb2D.position.y, 0), Time.deltaTime*_rotSmoothness);

        _thoughtExplosionTimer = Mathf.Max(_thoughtExplosionTimer-Time.deltaTime, 0);
        _thoughtStunCoolDownTimer = Mathf.Max(_thoughtStunCoolDownTimer-Time.deltaTime, 0);

        if(Vector2.Distance(_player.position, _rb2D.position) <= _thoughtExplosionRadius-0.25f && _thoughtExplosionTimer == 0) {
            _thoughtExplosionTimer = 2f;
            _animator.Play("Explosion");
            _aiState = ConscienceAIState.THOUGHT_EXPLOSION;
        }

        if(Vector2.Distance(_player.position, _rb2D.position) <= _thoughtStunRadius-0.25f && _thoughtStunCoolDownTimer == 0) {
            _aiState = ConscienceAIState.THOUGHT_OVERLOAD;
            _animator.Play("Stun");
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

    public void ThoughtStun() {
        AudioManager.Instance.PlaySoundVaried(_stunSound);
        _thoughtStunParticles.Play();
        if(Vector2.Distance(_player.position, _rb2D.position) <= _thoughtStunRadius) {
            _playerHealth.Knockback(Vector2.zero, 0, __stunTime: _thoughtStunStunTime);
        }
    }

    public void StopThoughtStunning() {
        _thoughtStunCoolDownTimer = 5f+Random.Range(-1f, 1f);;
        _aiState = ConscienceAIState.TARGET;
    }

    public void AttackPlayer() {
        AudioManager.Instance.PlaySoundVaried(_explosionSound);
        _screenShaker.ShakeRandom(1.5f, 25f, 0.1f);
        GameObject explosion = Instantiate(_thoughtExplosionParticles, _rb2D.position, Quaternion.identity);
        Destroy(explosion, 3f);
        if(Vector2.Distance(_player.position, _rb2D.position) <= _thoughtExplosionRadius) {
            _playerHealth.TakeDamage(_thoughtExplosionDamage, _hittable, false);
            _playerHealth.Knockback((Vector2)_player.position-_rb2D.position, _thoughtExplosionKnockback, _thoughtExplosionStunTime);
        }
    }

    public void StopBiting() {
        _thoughtExplosionTimer = 2f+Random.Range(-0.5f, 0.5f);;
        _aiState = ConscienceAIState.TARGET;
    }

    public void Stun(float __duration) {
        _stunTimer = __duration;
        _animator.Play("Idle");
        if(_aiState == ConscienceAIState.THOUGHT_EXPLOSION) _thoughtExplosionTimer = 2f+Random.Range(-0.5f, 0.5f);
        if(_aiState == ConscienceAIState.THOUGHT_OVERLOAD) _thoughtStunCoolDownTimer = 2f+Random.Range(-0.5f, 0.5f);
        _aiState = ConscienceAIState.TARGET;
    }
}
