using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int _stage;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private BasicPlayerAttacks _playerAttacks;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerXP _playerXP;
    [SerializeField] private ScreenShaker _screenShaker;
    [SerializeField] private List<Item> _items = new List<Item>();
    [SerializeField] private TextMeshProUGUI _deathStats;

    public void OnPlayerDeath() {
        _deathStats.text = $"Kills: {PersistantStats.Instance.GetKills()}\nDamage Dealt: {PersistantStats.Instance.GetDamageDealt()}\nDamage Recieved: {PersistantStats.Instance.GetDamageRecieved()}\nStage Reached: {_stage}";
    }

    public PlayerHealth GetPlayerHealth() {
        return _playerHealth;
    }

    public BasicPlayerAttacks GetPlayerAttacks() {
        return _playerAttacks;
    }

    public PlayerMovement GetPlayerMovement() {
        return _playerMovement;
    }

    public PlayerXP GetPlayerXP() {
        return _playerXP;
    }

    public ScreenShaker GetScreenShaker() {
        return _screenShaker;
    }

    private void Start() {
        _items = PersistantItemsManager.Instance.GetItems();
    }

    private void Update() {
        foreach(Item item in _items) {
            item.Update(this, _stage);
        }
    }

    public void OnPlayerPrimaryAttack() {
        foreach(Item item in _items) {
            item.OnPlayerPrimaryAttack(this, _stage);
        }
    }

    public void OnPlayerSecondaryAttack() {
        foreach(Item item in _items) {
            item.OnPlayerSecondaryAttack(this, _stage);
        }
    }

    public void OnPlayerSpecialAttack() {
        foreach(Item item in _items) {
            item.OnPlayerSpecialAttack(this, _stage);
        }
    }

    public void OnEnemyHit(Hittable __hittable, List<Item>  __itemChain) {
        foreach(Item item in _items) {
            if(!__itemChain.Contains(item)) item.OnEnemyHit(this, _stage, __hittable, __itemChain);
        }
    }

    public void OnEnemyHit(Hittable __hittable) {
        foreach(Item item in _items) {
            item.OnEnemyHit(this, _stage, __hittable, new List<Item>());
        }
    }

    public void OnInflictDamage(ref float __damage, List<Item> __itemChain) {
        foreach(Item item in _items) {
            if(!__itemChain.Contains(item)) item.OnInflictDamage(this, _stage, ref __damage, __itemChain);
        }
        PersistantStats.Instance.SetDamageDealt(PersistantStats.Instance.GetDamageDealt()+__damage);
    }

    public void OnInflictDamage(ref float __damage) {
        foreach(Item item in _items) {
            item.OnInflictDamage(this, _stage, ref __damage, new List<Item>());
        }
        PersistantStats.Instance.SetDamageDealt(PersistantStats.Instance.GetDamageDealt()+__damage);
    }

    public void OnPlayerDamaged(ref float __damage, List<Item> __itemChain) {
        foreach(Item item in _items) {
            if(!__itemChain.Contains(item)) item.OnPlayerDamaged(this, _stage, ref __damage, __itemChain);
        }
        PersistantStats.Instance.SetDamageRecieved(PersistantStats.Instance.GetDamageRecieved()+__damage);
    }

    public void OnPlayerDamaged(ref float __damage) {
        foreach(Item item in _items) {
            item.OnPlayerDamaged(this, _stage, ref __damage, new List<Item>());
        }
        PersistantStats.Instance.SetDamageRecieved(PersistantStats.Instance.GetDamageRecieved()+__damage);
    }

    public void OnPlayerHit(Hittable __hittable, List<Item> __itemChain) {
        foreach(Item item in _items) {
            if(!__itemChain.Contains(item)) item.OnPlayerHit(this, __hittable, _stage, __itemChain);
        }
    }

    public void OnPlayerHit(Hittable __hittable) {
        foreach(Item item in _items) {
            item.OnPlayerHit(this, __hittable, _stage, new List<Item>());
        }
    }

    public void OnPlayerHealed(ref float __health, List<Item> __itemChain) {
        foreach(Item item in _items) {
            if(!__itemChain.Contains(item)) item.OnPlayerHealed(this, _stage, ref __health, __itemChain);
        }
    }

    public void OnPlayerHealed(ref float __health) {
        foreach(Item item in _items) {
            item.OnPlayerHealed(this, _stage, ref __health, new List<Item>());
        }
    }

    public void OnPlayerKnockedback(ref float __power, List<Item> __itemChain) {
        foreach(Item item in _items) {
            if(!__itemChain.Contains(item)) item.OnPlayerKnockedback(this, _stage, ref __power, __itemChain);
        }
    }

    public void OnPlayerKnockedback(ref float __power) {
        foreach(Item item in _items) {
            item.OnPlayerKnockedback(this, _stage, ref __power, new List<Item>());
        }
    }

    public void OnEnemyKill() {
        foreach(Item item in _items) {
            item.OnEnemyKill(this, _stage);
        }
        PersistantStats.Instance.SetKills(PersistantStats.Instance.GetKills()+1);
    }

}
