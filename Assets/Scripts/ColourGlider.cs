using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColourGlider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private float _timePerCycle;
    private float _timer;
    void Update() {
        _timer += Time.deltaTime;
        if(_timer >= _timePerCycle) {
            _timer -= _timePerCycle;
        }
        _text.color = _gradient.Evaluate(_timer/_timePerCycle);
        
    }
}
