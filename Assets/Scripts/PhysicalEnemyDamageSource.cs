using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalEnemyDamageSource : MonoBehaviour
{
    private Hittable _hittable;
    private float _damage;
    private bool _destroyOnHit;
    private float _knockback;
    private float _stunTime;
    public void SetData(float __damage, float __knockback, float __stunTime, Hittable __hittable, float __destroyTime, bool __destroyOnHit) {
        _damage = __damage;
        _knockback = __knockback;
        _stunTime = __stunTime;
        if(__destroyTime > 0) {
            Destroy(gameObject, __destroyTime);
        }
        _destroyOnHit = __destroyOnHit;
        _hittable = __hittable;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        PlayerHealth Playerhit = other.GetComponent<PlayerHealth>();
        if(Playerhit) {
            Playerhit.TakeDamage(_damage, _hittable, false);
            Playerhit.Knockback((Vector2)(Playerhit.transform.position-transform.position), _knockback, _stunTime);
            if(_destroyOnHit) Destroy(gameObject);
        }
    }
}
