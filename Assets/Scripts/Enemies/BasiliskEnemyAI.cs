using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BasiliskEnemyAI : MonoBehaviour, EnemyAI
{
    private enum BasiliskAIState {
        TARGET,
        GLARE,
        BITE
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

    [Header("Basilisk AI")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _biteDamage;
    [SerializeField] private float _biteRadius;
    [SerializeField] private float _biteKnockback;
    [SerializeField] private float _biteStunTime;
    [SerializeField] private float _glareDamagePerSecond;
    [SerializeField] private float _glareStartRadius;
    [SerializeField] private float _glareStunTime;
    [SerializeField] private GlareCone _glareCone;
    [SerializeField] private float _glareSmoothness;
    [SerializeField] private float _rotSmoothness;
    [SerializeField] private Hittable _hittable;
    private float _biteTimer;
    private float _glareTimer;
    private float _stunTimer;
    private BasiliskAIState _aiState = BasiliskAIState.TARGET;
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
        _glareCone.SetData(_glareDamagePerSecond, _glareStunTime, this);
        _glareTimer = 3f;
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
            case BasiliskAIState.TARGET:
                TargetAI();
                break;
            case BasiliskAIState.GLARE:
                GlareAI();
                break;
            case BasiliskAIState.BITE:
                _rb2D.velocity = Vector2.zero;
                break;
        }
    }

    private void GlareAI() {
        _rb2D.velocity = Vector2.zero;
        transform.right = Vector3.Lerp(transform.right, new Vector3(_player.position.x, _player.position.y, 0)-new Vector3(_rb2D.position.x, _rb2D.position.y, 0), Time.deltaTime*_glareSmoothness);
    }

    private void TargetAI() {
        //transform.right = Vector3.Lerp(transform.right, new Vector3(_player.position.x, _player.position.y, 0)-new Vector3(_rb2D.position.x, _rb2D.position.y, 0), Time.deltaTime*_rotSmoothness);

        _biteTimer = Mathf.Max(_biteTimer-Time.deltaTime, 0);
        _glareTimer = Mathf.Max(_glareTimer-Time.deltaTime, 0);

        if(Vector2.Distance(_player.position, _rb2D.position+(Vector2)transform.right) <= _biteRadius-0.15f && _biteTimer == 0) {
            _biteTimer = 1f;
            _animator.Play("Bite");
            _aiState = BasiliskAIState.BITE;
        }

        if(Vector2.Distance(_player.position, _rb2D.position+(Vector2)transform.right) <= _glareStartRadius && _glareTimer == 0) {
            _glareTimer = 5f;
            _animator.Play("Glare");
            _aiState = BasiliskAIState.GLARE;
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
        if(Vector2.Distance(_player.position, _rb2D.position) <= _biteRadius) {
            _playerHealth.TakeDamage(_biteDamage, _hittable, false);
            _playerHealth.Knockback((Vector2)_player.position-_rb2D.position, _biteKnockback, _biteStunTime);
        }
    }

    public void StopBiting() {
        _biteTimer = 1f;
        _aiState = BasiliskAIState.TARGET;
    }

    public void StopGlaring() {
        _glareTimer = 3f+Random.Range(-0.5f, 0.5f);
        _glareCone.gameObject.SetActive(false);
        _aiState = BasiliskAIState.TARGET;
    }

    public void StartGlaring() {
        _glareCone.gameObject.SetActive(true);
        Invoke("StopGlaring", 2f);
    }

    public void Stun(float __duration) {
        _stunTimer = __duration;
        if(_aiState == BasiliskAIState.BITE) _biteTimer = 1f;
        if(_aiState == BasiliskAIState.GLARE) _glareTimer = 3f;
        _aiState = BasiliskAIState.TARGET;
        _animator.Play("Idle");
    }
}
