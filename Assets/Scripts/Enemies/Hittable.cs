using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Hittable : MonoBehaviour
{
    [SerializeField] private Animator _universeDeathAnimator;
    [SerializeField] private Sound _enemyHit;
    [SerializeField] private float _maxHealth;
    [SerializeField] private PopUpTextCreator _popUpTextCreator;
    [SerializeField] private Rigidbody2D _rb2D;
    [SerializeField] private float _stunEfficient;
    [SerializeField] private float _xpGiven;
    [SerializeField] private float _xpVariance;
    [SerializeField] private float _percentageDefense;
    private EnemyAI _enemyAI;
    private float _health;
    private bool _dead;

    private void Start() {
        _health = _maxHealth;
        _enemyAI = GetComponent<EnemyAI>();
        _dead = false;
    }

    public void SetData(PopUpTextCreator __popUpTextCreator) {
        _popUpTextCreator = __popUpTextCreator;
    }

    public void TakeDamage(float __damage, PlayerXP __playerXP) {
        if(_dead) return;
        AudioManager.Instance.PlaySoundVaried(_enemyHit);
        float appliedDamage = Mathf.Max(__damage*(1-_percentageDefense), 1);
        _health -= appliedDamage;
        _popUpTextCreator.SpawnPopUpText($"<color=red>{appliedDamage}</color>", new Vector2(transform.position.x, transform.position.y));
        if(_health <= 0) {
            _dead = true;
            StopAllCoroutines();
            if(transform.CompareTag("Universe")) {
                _universeDeathAnimator.CrossFade("Death", 0.5f);
                return;
            }
            if(__playerXP) __playerXP.GetComponent<PlayerStats>().OnEnemyKill();
            float givenXP = _xpGiven+Random.Range(-_xpVariance, _xpVariance);
            if(__playerXP) __playerXP.AddXP(givenXP);
            if(givenXP > 0) _popUpTextCreator.SpawnPopUpText($"<color=green>{givenXP:0}</color>", new Vector2(transform.position.x, transform.position.y));
            Destroy(gameObject);
        }
    }

    public void TakeDamageWithColour(float __damage, PlayerXP __playerXP, string __colourTag) {
        if(_dead) return;
        AudioManager.Instance.PlaySoundVaried(_enemyHit);
        float appliedDamage = Mathf.Max(__damage*(1-_percentageDefense), 1);
        _health -= appliedDamage;
        _popUpTextCreator.SpawnPopUpText($"<{__colourTag}>{appliedDamage}</color>", new Vector2(transform.position.x, transform.position.y));
        if(_health <= 0) {
            _dead = true;
            StopAllCoroutines();
            if(transform.CompareTag("Universe")) {
                _universeDeathAnimator.CrossFade("Death", 0.5f);
                return;
            }
            if(__playerXP) __playerXP.GetComponent<PlayerStats>().OnEnemyKill();
            float givenXP = _xpGiven+Random.Range(-_xpVariance, _xpVariance);
            if(__playerXP) __playerXP.AddXP(givenXP);
            if(givenXP > 0) _popUpTextCreator.SpawnPopUpText($"<color=green>{givenXP:0}</color>", new Vector2(transform.position.x, transform.position.y));
            Destroy(gameObject);
        }
    }

    public void Heal(float __heal) {
        _health = Mathf.Max(_health+__heal, _maxHealth/2f);
    }

    public void Knockback(Vector2 __dir, float __power, float __stunTime) {
        _enemyAI.Stun(__stunTime*_stunEfficient);
        _rb2D.bodyType = RigidbodyType2D.Dynamic;
        _rb2D.AddForce(__dir*__power, ForceMode2D.Impulse);
    }

    public float GetHealth() {
        return _health;
    }

    public float GetMaxHealth() {
        return _maxHealth;
    }

    public void SetDefensePercentage(float __perc) {
        _percentageDefense = __perc;
    }

    public void StartBurn(PlayerStats __playerStats, int __stage) {
        StopAllCoroutines();
        StartCoroutine(Burn(__playerStats, __stage));
    }

    private IEnumerator Burn(PlayerStats __playerStats, int __stage) {
        for(int i=0; i<40; i++) {
            TakeDamageWithColour(__stage, __playerStats.GetPlayerXP(), "color=orange");
            yield return new WaitForSeconds(0.1f);
        }
    }
}
