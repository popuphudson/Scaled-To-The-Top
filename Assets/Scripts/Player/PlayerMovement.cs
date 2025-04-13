using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private Rigidbody2D _rb2D;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _dashDistance;
    [SerializeField] private float _dashCoolDown;
    [SerializeField] private LayerMask _obsticalLayer;
    [SerializeField] private Sound _dashSound;
    private Vector2 _moveDir;
    private Vector2 _pullInfluence;
    private Camera _camera;
    private float _stunTimer;
    private float _lookEfficient;
    private bool _stopInput = false;
    private float _dashCoolDownTimer = 0;
    private bool _stopLooking = false;

    public void KilledEnemy() {
        _dashCoolDownTimer -= 1;
    }

    private void Start() {
        _camera = Camera.main;
        _lookEfficient = -1;
    }

    private void Update() {
        if(_stopInput) return;
        _stunTimer = Mathf.Max(_stunTimer-Time.deltaTime, 0);
        _moveDir = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
        if(Input.GetKeyDown(KeyCode.LeftShift) && _dashCoolDownTimer <= 0) {
            AudioManager.Instance.PlaySoundVaried(_dashSound);
            RaycastHit2D hit2D = Physics2D.Linecast(transform.position, transform.position+(transform.right*_dashDistance), _obsticalLayer);
            if(hit2D) transform.position = hit2D.point;
             else transform.position += transform.right*_dashDistance;
            _dashCoolDownTimer = _dashCoolDown;
        }
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition); 
        mousePos.z = 0;
        if(!_stopLooking) {
            if(_lookEfficient <= 0) transform.right = (mousePos-transform.position).normalized;
            else transform.right = transform.right = Vector2.Lerp(transform.right, (mousePos-transform.position).normalized, _lookEfficient*Time.deltaTime);
        }
    }

    private void FixedUpdate() {
        if(_stunTimer == 0) {
            _rb2D.velocity = (_moveDir*_movementSpeed)+_pullInfluence;
        }
    }

    public void Knockback(Vector2 __dir, float __power, float __stunTime) {
        _stunTimer = __stunTime;
        _rb2D.velocity = Vector2.zero;
        _rb2D.AddForce(__dir*__power, ForceMode2D.Impulse);
    }

    public void SetLookEfficient(float __lookEfficient) {
        _lookEfficient = __lookEfficient;
    }


    public void StopInput() {
        _stopInput = true;
    }

    public Vector2 GetVelocity() {
        return _rb2D.velocity;
    }

    public Vector2 GetMovementVelocity() {
        if(_stunTimer == 0) return _moveDir*_movementSpeed;
        return Vector2.zero;
    }

    public float GetDashCoolDownTimer() {
        return _dashCoolDownTimer;
    }

    public float GetDashCoolDown() {
        return _dashCoolDown;
    }

    public void PullTowards(Vector2 __pos, float __strength) {
        _pullInfluence = (__pos-(Vector2)transform.position)*__strength;
    }

    public void StopPulling() {
        _pullInfluence = Vector2.zero;
    }

    public void SetLooking(bool __canLook) {
        _stopLooking = !__canLook;
    }
}
