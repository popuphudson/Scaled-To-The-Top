using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Passed Data")]
    [SerializeField] private PopUpTextCreator _popUpTextCreator;
    [SerializeField] private Transform _player;
    [SerializeField] private ScreenShaker _screenShaker;
    [Header("Enemies")]
    [SerializeField] private GameObject[] _enemies;
    [SerializeField] private int[] _enemyWeights;
    [SerializeField] private float[] _enemySpawnImpact;
    [Header("Spawning")]
    [SerializeField] private Vector2 _topLeftCorner;
    [SerializeField] private Vector2 _bottomRightCorner;
    [SerializeField] private float _enemySpawnTime;
    [SerializeField] private float _maxEnemies = 30;
    private float _enemySpawnTimer;
    private int _totalEnemyWeight;

    private void Start() {
        AudioManager.Instance.FadeToSong(1, 1.5f);
        _enemySpawnTimer = _enemySpawnTime+5f;
        _totalEnemyWeight = 0;
        foreach(int weight in _enemyWeights) {
            _totalEnemyWeight += weight;
        }
    }

    private void Update() {
        _enemySpawnTimer = Mathf.Max(_enemySpawnTimer-(Time.deltaTime*(1.5f-(transform.childCount/_maxEnemies))), 0);
        if(_enemySpawnTimer == 0 && transform.childCount < _maxEnemies) {
            _enemySpawnTimer = _enemySpawnTime;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy() {
        Hittable hittable = Instantiate(RandomEnemy(), transform).GetComponent<Hittable>();
        hittable.transform.position = RandomLocation();
        hittable.SetData(_popUpTextCreator);
        hittable.GetComponent<EnemyAI>().SetData(_player, _screenShaker);
    }

    private Vector2 RandomLocation() {
        return new Vector2(
            Random.Range(_topLeftCorner.x, _bottomRightCorner.x),
            Random.Range(_topLeftCorner.y, _bottomRightCorner.y)
        );
    }

    private GameObject RandomEnemy() {
        int chance = Random.Range(0, _totalEnemyWeight);
        int total = 0;
        for(int i=0; i<_enemies.Length; i++) {
            if(chance-total <= _enemyWeights[i]) {
                _enemySpawnTimer += Random.Range(0f, _enemySpawnImpact[i]);
                return _enemies[i];
            }
            total += _enemyWeights[i];
        }
        return _enemies[_enemies.Length-1];
    }
}
