using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DimensionHopperEnemyAI : MonoBehaviour, EnemyAI
{
    private enum DimensionHopperAIState {
        TARGET,
        TELEPORTING,
        PUNCHING
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

    [Header("Dimension Hopper AI")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _punchDamage;
    [SerializeField] private float _punchRadius;
    [SerializeField] private float _punchKnockback;
    [SerializeField] private float _punchStunTime;
    [SerializeField] private float _teleportRadius;
    [SerializeField] private float _teleportCoolDown;
    [SerializeField] private float _rotSmoothness;
    [SerializeField] private Hittable _hittable;
    [SerializeField] private Sound _tpSound;
    private float _teleportCoolDownTimer;
    private float _punchTimer;
    private float _stunTimer;
    private DimensionHopperAIState _aiState = DimensionHopperAIState.TARGET;
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
        _teleportCoolDownTimer = _teleportCoolDown;
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
            case DimensionHopperAIState.TARGET:
                TargetAI();
                break;
            case DimensionHopperAIState.TELEPORTING:
                _rb2D.velocity = Vector2.zero;
                break;
            case DimensionHopperAIState.PUNCHING:
                _rb2D.velocity = Vector2.zero;
                break;
        }
    }

    private void TargetAI() {
        //transform.right = Vector3.Lerp(transform.right, new Vector3(_player.position.x, _player.position.y, 0)-new Vector3(_rb2D.position.x, _rb2D.position.y, 0), Time.deltaTime*_rotSmoothness);

        _punchTimer = Mathf.Max(_punchTimer-Time.deltaTime, 0);
        _teleportCoolDownTimer = Mathf.Max(_teleportCoolDownTimer-Time.deltaTime, 0);

        if(Vector2.Distance(_player.position, _rb2D.position) <= _punchRadius-0.15f && _punchTimer == 0) {
            _punchTimer = 1.25f;
            _animator.Play("Punch");
            _aiState = DimensionHopperAIState.PUNCHING;
        }

        if(_teleportCoolDownTimer == 0) {
            _aiState = DimensionHopperAIState.TELEPORTING;
            _animator.Play("Teleport");
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

    private void Teleport() {
        AudioManager.Instance.PlaySoundVaried(_tpSound);
        Vector2 randomPos = Random.insideUnitCircle*_teleportRadius;
        RaycastHit2D hit2D = Physics2D.Linecast(_rb2D.position, randomPos, _obsticalLayer);
        if(hit2D) {
            randomPos = hit2D.point;
        }
        _rb2D.position = randomPos;
    }

    private void StopTeleport() {
        _aiState = DimensionHopperAIState.TARGET;
        _teleportCoolDownTimer = _teleportCoolDown+Random.Range(-0.25f, 0.25f);
        if(Random.Range(0, 100) < 20) {
            _aiState = DimensionHopperAIState.TELEPORTING;
            _animator.Play("Teleport");
        }
    }

    public void AttackPlayer() {
        if(Vector2.Distance(_player.position, _rb2D.position) <= _punchRadius) {
            _playerHealth.TakeDamage(_punchDamage, _hittable, false);
            _playerHealth.Knockback((Vector2)_player.position-_rb2D.position, _punchKnockback, _punchStunTime);
        }
    }

    public void StopBiting() {
        _punchTimer = 1.25f+Random.Range(-0.25f, 0.25f);;
        _aiState = DimensionHopperAIState.TARGET;
    }

    public void Stun(float __duration) {
        _stunTimer = __duration;
        _animator.Play("Idle");
        if(_aiState == DimensionHopperAIState.PUNCHING) _punchTimer = 1.25f+Random.Range(-0.25f, 0.25f);;
        _aiState = DimensionHopperAIState.TARGET;
    }
}
