using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiredFist : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rb2D;
    [SerializeField] private GameObject _exploadParticles;
    private PlayerStats _playerStats;
    private List<Item> _itemChain;
    private float _damage;
    private int _pierce;
    private Vector2 _dir;
    private float _destroyTimer = 0;
    private bool _destroy;

    public void SetData(float __damage, int __pierce, Vector2 __dir, PlayerStats __playerStats, List<Item> __itemChain, float __destroyTimer) {
        _playerStats = __playerStats;
        _itemChain= __itemChain;
        _damage = __damage;
        _dir = __dir;
        _pierce = __pierce;
        _destroyTimer = __destroyTimer;
        if(_destroyTimer > 0) _destroy = true;
    }

    private void Update() {
        _rb2D.velocity = _dir*_speed;
        transform.right = _dir;
        _destroyTimer -= Time.deltaTime;
        if(_destroyTimer <= 0 && _destroy) GetRidOfMe();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Hittable hittable = other.GetComponent<Hittable>();
        if(hittable) {
            _pierce--;
            float inflictedDamage = _damage;
            _playerStats.OnInflictDamage(ref inflictedDamage, _itemChain);
            _playerStats.OnEnemyHit(hittable, _itemChain);
            hittable.TakeDamage(inflictedDamage, _playerStats.GetPlayerXP());
            _playerStats.GetScreenShaker().ShakeDirectionOnce(transform.right, 1f, 10f, 0.25f);
            if(_pierce < 0) GetRidOfMe();
        }
    }

    private void GetRidOfMe() {
        GameObject explosion = Instantiate(_exploadParticles, transform.position, Quaternion.identity);
        Destroy(explosion, 5f);
        Destroy(gameObject);
    }
}
