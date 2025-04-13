using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statText;
    public void BackToMenu() {
        SceneManager.LoadScene(0);
    }

    private void Start() {
        AudioManager.Instance.FadeToSong(3, 1.5f);
        _statText.text = $"Kills: {PersistantStats.Instance.GetKills()}\nDamage Dealt: {PersistantStats.Instance.GetDamageDealt()}\nDamage Recieved: {PersistantStats.Instance.GetDamageRecieved()}";
    }
}
