using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlanetaryConstrutorEnemyAI : MonoBehaviour, EnemyAI
{
    private enum PlanetaryConstrutorAIState {
        TARGET,
        PLANET_THROW
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
    [SerializeField] private GameObject _planetThrowable;
    [SerializeField] private float _planetThrowDamage;
    [SerializeField] private float _planetThrowRadius;
    [SerializeField] private float _planetThrowKnockback;
    [SerializeField] private float _planetThrowStunTime;
    [SerializeField] private float _planetThrowForce;
    [SerializeField] private float _rotSmoothness;
    [SerializeField] private Hittable _hittable;
    [SerializeField] private Sound _throwSound;
    private float _planetThrowTimer;
    private float _stunTimer;
    private PlanetaryConstrutorAIState _aiState = PlanetaryConstrutorAIState.TARGET;
    private ScreenShaker _screenShaker;

    public void SetData(Transform __player, ScreenShaker __screenShaker) {
        _player = __player;
        _screenShaker = __screenShaker;
    }

    private void Start() {
        _seeker = GetComponent<Seeker>();
        _rb2D = GetComponent<Rigidbody2D>();
        
        InvokeRepeating("UpdatePathing", 0f, 0.5f);
        _planetThrowTimer = 1f;
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
            case PlanetaryConstrutorAIState.TARGET:
                TargetAI();
                break;
            case PlanetaryConstrutorAIState.PLANET_THROW:
                _rb2D.velocity = Vector2.zero;
                break;
        }
    }

    private void TargetAI() {
        transform.right = Vector3.Lerp(transform.right, new Vector3(_player.position.x, _player.position.y, 0)-new Vector3(_rb2D.position.x, _rb2D.position.y, 0), Time.deltaTime*_rotSmoothness);

        _planetThrowTimer = Mathf.Max(_planetThrowTimer-Time.deltaTime, 0);

        if(Vector2.Distance(_player.position, _rb2D.position) <= _planetThrowRadius && _planetThrowTimer == 0) {
            _planetThrowTimer = 2f;
            _animator.Play("Throw");
            _aiState = PlanetaryConstrutorAIState.PLANET_THROW;
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
        _screenShaker.ShakeRandom(0.75f, 25f, 0.1f);
        PhysicalEnemyDamageSource damageSource = Instantiate(_planetThrowable, _rb2D.position, Quaternion.identity).GetComponent<PhysicalEnemyDamageSource>();
        damageSource.transform.right = transform.right;
        damageSource.SetData(_planetThrowDamage, _planetThrowKnockback, _planetThrowStunTime, _hittable, 3f, true);
        damageSource.transform.position += damageSource.transform.right*0.5f;
        damageSource.GetComponent<Rigidbody2D>().AddForce(damageSource.transform.right*_planetThrowForce, ForceMode2D.Impulse);
    }

    public void StopAttacking() {
        _planetThrowTimer = 1f+Random.Range(-0.5f, 0.5f);
        _aiState = PlanetaryConstrutorAIState.TARGET;
    }

    public void Stun(float __duration) {
        _stunTimer = __duration;
        if(_aiState == PlanetaryConstrutorAIState.PLANET_THROW) _planetThrowTimer = 2f+Random.Range(-0.5f, 0.5f);
        _aiState = PlanetaryConstrutorAIState.TARGET;
        _animator.Play("Idle");
    }
}
