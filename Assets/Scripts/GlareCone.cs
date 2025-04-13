using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlareCone : MonoBehaviour
{
    private BasiliskEnemyAI _basiliskEnemyAI;
    private Hittable _hittable;
    private float _damage;
    private float _stunTime;
    private bool _inGlare;
    private PlayerHealth _playerHealth;
    public void SetData(float __damage, float __stunTime, BasiliskEnemyAI __enemyAI) {
        _damage = __damage;
        _stunTime = __stunTime;
        _basiliskEnemyAI = __enemyAI;
        _hittable = _basiliskEnemyAI.GetComponent<Hittable>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        PlayerHealth playerHit = other.GetComponent<PlayerHealth>();
        if(playerHit) {
            _playerHealth = playerHit;
            _inGlare = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        PlayerHealth playerHit = other.GetComponent<PlayerHealth>();
        if(playerHit) {
            _inGlare = false;
        }
    }

    private void Update() {
        if(_inGlare) {
            float myAngle = Mathf.Atan2(-_basiliskEnemyAI.transform.right.y, -_basiliskEnemyAI.transform.right.x)*Mathf.Rad2Deg;
            float playerAngle = Mathf.Atan2(_playerHealth.transform.right.y, _playerHealth.transform.right.x)*Mathf.Rad2Deg;
            if(playerAngle >= myAngle-15f && playerAngle <= myAngle+15f) {
                _playerHealth.TakeDamage(_damage*Time.deltaTime, _hittable, true);
                _playerHealth.Knockback(Vector2.zero, 0, _stunTime);
            }
        }
    }
}
