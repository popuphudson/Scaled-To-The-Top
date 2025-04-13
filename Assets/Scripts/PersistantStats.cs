using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistantStats : MonoBehaviour
{
    public static PersistantStats Instance;
    private int _kills;
    private float _damageDealt;
    private float _damageRecieved;
    private int _stageReached;

    private void Awake() {
        if(Instance) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public int GetKills() {
        return _kills;
    }

    public float GetDamageDealt() {
        return _damageDealt;
    }

    public float GetDamageRecieved() {
        return _damageRecieved;
    }

    public int GetStageReached() {
        return _stageReached;
    }

    public void SetKills(int __kills) {
        _kills = __kills;
    }

    public void SetDamageDealt(float __damageDealt) {
        _damageDealt = __damageDealt;
    }

    public void SetDamageRecieved(float __damageRecieved) {
        _damageRecieved = __damageRecieved;
    }

    public void SetStageReached(int __stageReached) {
        _stageReached = __stageReached;
    }
}
