using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalPlayerDamageSource : MonoBehaviour
{
    [SerializeField] private AttackType _attackType;
    [SerializeField] private BasicPlayerAttacks _playerAttacks;
    private void OnTriggerEnter2D(Collider2D other) {
        Hittable hit = other.GetComponent<Hittable>();
        if(hit) {
            switch(_attackType) {
                case AttackType.PRIMARY:
                    _playerAttacks.DoDamageWithPrimary(hit);
                    break;
                case AttackType.SECONDARY:
                    _playerAttacks.DoDamageWithSecondary(hit);
                    break;
                case AttackType.SPECIAL:
                    _playerAttacks.DoDamageWithSpecial(hit);
                    break;   
            }
        }
    }
}
