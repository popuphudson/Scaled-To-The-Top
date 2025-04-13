using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpText : MonoBehaviour
{
    private Vector2 _moveDir;
    public void SetMoveDir(Vector2 __moveDir) {
        _moveDir = __moveDir;
    }
    void Start() {
        Destroy(gameObject, 2f);
    }

    private void Update() {
        transform.position += new Vector3(_moveDir.x, _moveDir.y, 0)*Time.deltaTime;
    }
}
