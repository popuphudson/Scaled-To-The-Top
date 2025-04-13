using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfCharge : MonoBehaviour
{
    private WolfEnemyAI _wolfEnemyAI;
    private Hittable _hittable;
    private float _damage;
    private float _knockback;
    private float _stunTime;
    public void SetData(float __damage, float __knockback, float __stunTime, WolfEnemyAI __enemyAI) {
        _damage = __damage;
        _knockback = __knockback;
        _stunTime = __stunTime;
        _wolfEnemyAI = __enemyAI;
        _hittable = _wolfEnemyAI.GetComponent<Hittable>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        PlayerHealth Playerhit = other.GetComponent<PlayerHealth>();
        if(Playerhit) {
            Playerhit.TakeDamage(_damage, _hittable, false);
            Playerhit.Knockback((Vector2)(Playerhit.transform.position-transform.position), _knockback, _stunTime);
            _wolfEnemyAI.StopCharge();
        }
    }
}
