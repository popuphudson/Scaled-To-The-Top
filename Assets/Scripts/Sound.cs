using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "Sound", order = 0)]
public class Sound : ScriptableObject {
    public AudioClip Clip;
    [Range(0, 1)]
    public float Volume = 0.5f;
    [Range(-3, 3)]
    public float Pitch = 1f;
}