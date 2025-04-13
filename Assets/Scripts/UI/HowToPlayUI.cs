using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayUI : MonoBehaviour
{
    [SerializeField] private Sound _selectSound;
    [SerializeField] private GameObject _abilityText, _doingText;
    [SerializeField] private GameObject _howDoIPlayMenu;

    public void WhatAmIDoing() {
        AudioManager.Instance.PlaySoundVaried(_selectSound);
        _doingText.SetActive(true);
        _howDoIPlayMenu.SetActive(false);
    }

    public void Ability() {
        AudioManager.Instance.PlaySoundVaried(_selectSound);
        _abilityText.SetActive(true);
        _howDoIPlayMenu.SetActive(false);
    }

    public void BackToMain() {
        AudioManager.Instance.PlaySoundVaried(_selectSound);
        _doingText.SetActive(false);
        _abilityText.SetActive(false);
        _howDoIPlayMenu.SetActive(true);
    }
}
