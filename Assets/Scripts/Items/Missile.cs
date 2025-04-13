using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb2D;
    [SerializeField] private float _rotSmoothness;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private GameObject _deathPartices;
    private Hittable _target;
    private float _damage;
    private PlayerStats _playerStats;
    private List<Item> _itemChain;
    public void SetTarget(Hittable __target) {
        _target = __target;
    }
    public void SetData(float __damage, PlayerStats __playerStats, List<Item> __itemChain) {
        _damage = __damage;
        _playerStats = __playerStats;
        _itemChain = __itemChain;
    }

    private void Update() {
        if(_target == null) {
            GameObject particles = Instantiate(_deathPartices, transform.position, Quaternion.identity);
            Destroy(particles, 5f);
            Destroy(gameObject);
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2);
            foreach(Collider2D hit in hits) {
                Hittable enemyHit = hit.GetComponent<Hittable>();
                if(enemyHit) {
                    float inflictedDamage = _damage/2f;
                    _playerStats.OnInflictDamage(ref inflictedDamage, _itemChain);
                    _playerStats.OnEnemyHit(enemyHit, _itemChain);
                    enemyHit.TakeDamageWithColour(inflictedDamage, _playerStats.GetPlayerXP(), "color=#aaaa00ff");
                }
            }
            return;
        }
        transform.up = Vector2.Lerp(transform.up, (_target.transform.position-transform.position).normalized, Time.deltaTime*_rotSmoothness);
        _rb2D.velocity = transform.up*_moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Hittable>() == _target) {
            GameObject particles = Instantiate(_deathPartices, transform.position, Quaternion.identity);
            Destroy(particles, 5f);
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.75f);
            float inflictedDamage = 0;
            foreach(Collider2D hit in hits) {
                Hittable enemyHit = hit.GetComponent<Hittable>();
                if(enemyHit) {
                    inflictedDamage = _damage/2f;
                    _playerStats.OnInflictDamage(ref inflictedDamage, _itemChain);
                    _playerStats.OnEnemyHit(enemyHit, _itemChain);
                    enemyHit.TakeDamageWithColour(inflictedDamage, _playerStats.GetPlayerXP(), "color=#aaaa00ff");
                }
            }
            inflictedDamage = _damage;
            if(_target) {
                _playerStats.OnInflictDamage(ref inflictedDamage, _itemChain);
                _playerStats.OnEnemyHit(_target, _itemChain);
                _target.TakeDamageWithColour(inflictedDamage, _playerStats.GetPlayerXP(), "color=#ffff00ff");
            }
            Destroy(gameObject);
        }
    }
}
