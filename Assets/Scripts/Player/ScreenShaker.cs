using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
    [SerializeField] private float _returnSpeed;
    [SerializeField] private Transform _playerGameHud;
    private Vector3 _origionalCameraPosition;
    private bool _shakeing;

    private void Start() {
        _origionalCameraPosition = transform.position;
    }

    public void ShakeRandom(float __magnitude, float __speed, float __duration) {
        StopAllCoroutines();
        StartCoroutine(ShakingRandomly(__magnitude/10f, __speed, __duration));
    }

    public void ShakeRandomOnce(float __magnitude, float __speed) {
        StopAllCoroutines();
        StartCoroutine(ShakeRandomlyOnce(__magnitude/10f, __speed));
    }

    public void ShakeDirectionOnce(Vector2 __dir, float __magnitude, float __speed, float __variance) {
        StopAllCoroutines();
        StartCoroutine(ShakeDirection(__dir, __magnitude/10f, __speed, __variance));
    }

    IEnumerator ShakeDirection(Vector2 __dir, float __magnitude, float __speed, float __variance) {
        _shakeing = true;
        Vector3 target = (__dir*__magnitude)+(Random.insideUnitCircle*__variance);
        target = target.normalized*Mathf.Min(target.magnitude, 2);
        target.z = -10f;
        while(true) {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime*__speed);
            _playerGameHud.localPosition = Vector2.Lerp(_playerGameHud.localPosition, target*50f, Time.deltaTime*__speed);
            if(Vector2.Distance(transform.position, target) < 0.1f) break;
            yield return null;
        }
        _shakeing = false;
    }

    IEnumerator ShakeRandomlyOnce(float __magnitude, float __speed) {
        _shakeing = true;
        Vector3 target = Random.insideUnitCircle.normalized*Random.Range(0.5f, 1f)*__magnitude;
        target = target.normalized*Mathf.Min(target.magnitude, 2);
        target.z = -10f;
        while(true) {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime*__speed);
            _playerGameHud.localPosition = Vector2.Lerp(_playerGameHud.localPosition, target*50f, Time.deltaTime*__speed);
            if(Vector2.Distance(transform.position, target) < 0.1f) break;
            yield return null;
        }
        _shakeing = false;
    }

    IEnumerator ShakingRandomly(float __magnitude, float __speed, float __duration) {
        _shakeing = true;
        float remaining = __duration;
        Vector3 target = Random.insideUnitCircle.normalized*Random.Range(0.5f, 1f)*__magnitude;
        target = target.normalized*Mathf.Min(target.magnitude, 2);
        target.z = -10f;
        while(remaining > 0) {
            if(Vector2.Distance(transform.position, target) < 0.1f) {
                target = Random.insideUnitCircle.normalized*Random.Range(0.5f, 1f)*__magnitude;
                target = target.normalized*Mathf.Min(target.magnitude, 2);
                target.z = -10f;
            }
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime*__speed);
            _playerGameHud.localPosition = Vector2.Lerp(_playerGameHud.localPosition, target*50f, Time.deltaTime*__speed);
            remaining -= Time.deltaTime;
            yield return null;
        }
        _shakeing = false;
    }

    private void Update() {
        if(!_shakeing) {
            transform.position = Vector3.Lerp(transform.position, _origionalCameraPosition, Time.deltaTime*_returnSpeed);
            _playerGameHud.localPosition = Vector2.Lerp(_playerGameHud.localPosition, Vector2.zero, Time.deltaTime*_returnSpeed);
        }
    }
}
