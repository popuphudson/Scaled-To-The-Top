using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDecay : MonoBehaviour
{
    [SerializeField] private LineRenderer _renderer;
    [SerializeField] private float _decayTime;
    private float _decayTimer = 1f;

    private void Start() {
        Destroy(gameObject, _decayTime+0.5f);
        _decayTimer = _decayTime;
    }
    
    private void Update() {
        _decayTimer -= Time.deltaTime;
        Color colour = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 1-(_decayTimer/_decayTime));
        Color c = _renderer.endColor;
		c.a = colour.a;
		_renderer.endColor = c;
    }
}
