using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpTextCreator : MonoBehaviour
{
    [SerializeField] private GameObject _popUpText, _popUpPlayerText;
    public void SpawnPopUpText(string __text, Vector2 __pos) {
        Vector2 moveDir = new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 2f));
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, (Mathf.Rad2Deg*Mathf.Atan2(moveDir.y, moveDir.x))-90));
        TextMeshPro popUpText = Instantiate(_popUpText, __pos, rotation).GetComponent<TextMeshPro>();
        popUpText.text = __text;
        popUpText.GetComponent<PopUpText>().SetMoveDir(moveDir);
    }

    public void SpawnPlayerPopUpText(string __text, Vector2 __pos) {
        Vector2 moveDir = new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 2f));
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, (Mathf.Rad2Deg*Mathf.Atan2(moveDir.y, moveDir.x))-90));
        TextMeshPro popUpText = Instantiate(_popUpPlayerText, __pos, rotation).GetComponent<TextMeshPro>();
        popUpText.text = __text;
        popUpText.GetComponent<PopUpText>().SetMoveDir(moveDir);
    }
}
