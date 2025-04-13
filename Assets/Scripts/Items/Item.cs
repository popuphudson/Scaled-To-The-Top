using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public abstract class Item {
    public abstract string GiveName();
    public abstract string GiveDescription();
    public virtual void Update(PlayerStats __playerStats, int __stage) {}
    public virtual void OnEnemyKill(PlayerStats __playerStats, int __stage) {}
    public virtual void OnPlayerPrimaryAttack(PlayerStats __playerStats, int __stage) {}
    public virtual void OnPlayerSecondaryAttack(PlayerStats __playerStats, int __stage) {}
    public virtual void OnPlayerSpecialAttack(PlayerStats __playerStats, int __stage) {}

    public virtual void OnEnemyHit(PlayerStats __playerStats, int __stage, Hittable __hittable, List<Item> __itemChain) {}
    public virtual void OnInflictDamage(PlayerStats __playerStats, int __stage, ref float __damage, List<Item> __itemChain) {}
    public virtual void OnPlayerHit(PlayerStats __playerStats, Hittable __hittable, int __stage, List<Item> __itemChain) {}
    public virtual void OnPlayerDamaged(PlayerStats __playerStats, int __stage, ref float __damage, List<Item> __itemChain) {}
    public virtual void OnPlayerHealed(PlayerStats __playerStats, int __stage, ref float __health, List<Item> __itemChain) {}
    public virtual void OnPlayerKnockedback(PlayerStats __playerStats, int __stage, ref float _knockback, List<Item> __itemChain) {}

}

public class DelayedHitItem : Item
{
    private float _delayedDamage;
    public override string GiveName() {
        return "Short Fuse Time Bomb";
    }

    public override string GiveDescription() {
        return "<color=lightblue>On hitting an enemy</color> deal <color=lightblue>75%</color> damage, however have a <color=green>20%</color> <color=grey>(+10% each growth)</color> chance to deal a delayed instance of damage for <color=lightblue>250%</color> damage of the damage that should have been delt.";
    }

    public override async void OnEnemyHit(PlayerStats __playerStats, int __stage, Hittable __hittable, List<Item> __itemChain) {
        float tempDamage = _delayedDamage;
        _delayedDamage = 0;
        if(tempDamage > 0 && Random.Range(0, 100) < 10+(10*__stage)) await DelayHit(__playerStats, __stage, tempDamage, __hittable, __itemChain);
        
    }

    private async Task DelayHit(PlayerStats __playerStats, int __stage, float __origionalHitDamage, Hittable __hittable, List<Item> __itemChain) {
        await Task.Delay(1500);
        if(__hittable) {
            __itemChain.Add(this);
            float inflictedDamage = __origionalHitDamage*2.5f;
            __playerStats.OnInflictDamage(ref inflictedDamage, __itemChain);
            __hittable.TakeDamageWithColour(inflictedDamage, __playerStats.GetPlayerXP(), "color=#550000ff");
            __playerStats.OnEnemyHit(__hittable, __itemChain);
            __playerStats.GetScreenShaker().ShakeRandom(inflictedDamage/10f, 50f, 0.1f);
        }
    }

    public override void OnInflictDamage(PlayerStats __playerStats, int __stage, ref float __damage, List<Item> __itemChain) {
        _delayedDamage = __damage;
        __damage *= 0.75f;
    }
}

public class FlameFuryItem : Item {
    public override string GiveName() {
        return "Sentient Flame";
    }

    public override string GiveDescription() {
        return "<color=lightblue>On hitting an enemy</color> inflict a burn that lasts <color=lightblue>4</color> seconds dealing <color=green>1</color> <color=grey>(+1 every growth)</color> damage every tick.";
    }

    public override void OnEnemyHit(PlayerStats __playerStats, int __stage, Hittable __hittable, List<Item> __itemChain) {
        __hittable.StartBurn(__playerStats, __stage);
    }
}

public class ReflectiveCoatingItem : Item
{
    public override string GiveName() {
        return "Reflective Plating";
    }

    public override string GiveDescription() {
        return "<color=lightblue>On recieving damage</color> block <color=green>20%</color> <color=grey>(+10% each growth)</color> and deal that blocked damage back at the enemy.";
    }

    private float _damageInflicted;

    public override void OnPlayerHit(PlayerStats __playerStats, Hittable __hittable, int __stage, List<Item> __itemChain) {
        if(__hittable) {
            float inflictedDamage = _damageInflicted;
            __itemChain.Add(this);
            __playerStats.OnInflictDamage(ref inflictedDamage, __itemChain);
            __hittable.TakeDamageWithColour(_damageInflicted, __playerStats.GetPlayerXP(), "color=#22ddffff");
            __playerStats.OnEnemyHit(__hittable, __itemChain);
            __playerStats.GetScreenShaker().ShakeRandom(_damageInflicted/10f, 50f, 0.1f);
            _damageInflicted = 0;
        }
    }

    public override void OnPlayerDamaged(PlayerStats __playerStats, int __stage, ref float __damage, List<Item> __itemChain) {
        _damageInflicted = __damage*(0.1f+(0.1f*__stage));
        __damage *= 1-(0.1f+(0.1f*__stage));
    }
}

public class MissileLauncherItem : Item
{
    public override string GiveName() {
        return "Missile Launcher";
    }

    public override string GiveDescription() {
        return "<color=lightblue>On hitting an enemy</color> have a <color=green>15%</color> <color=grey>(+5% each growth)</color> chance to fire a missile that deals <color=lightblue>250%</color> damage of the hit that causes the missile to fire, missiles expload when hitting an enemy or loosing tracking for <color=lightblue>50%</color> of the missile's damage.";
    }

    private float _dealtDamage;

    public override void OnEnemyHit(PlayerStats __playerStats, int __stage, Hittable __hittable, List<Item> __itemChain) {
        if(_dealtDamage == 0) return;
        if(Random.Range(0, 100) <= 10+(5*__stage)) SpawnMissile(__playerStats, __hittable, __itemChain);
    }

    private void SpawnMissile(PlayerStats __playerStats, Hittable __hittable, List<Item> __itemChain) {
        __itemChain.Add(this);
        GameObject missilePrefab = Resources.Load<GameObject>("ItemStuffs/Missile");
        GameObject missile = GameObject.Instantiate(missilePrefab, __playerStats.GetPlayerMovement().transform.position, Quaternion.identity);
        missile.GetComponent<Missile>().SetTarget(__hittable);
        missile.GetComponent<Missile>().SetData(_dealtDamage*2.5f, __playerStats, __itemChain);
        _dealtDamage = 0;
    }

    public override void OnInflictDamage(PlayerStats __playerStats, int __stage, ref float __damage, List<Item> __itemChain) {
        _dealtDamage = __damage;
    }
}

public class LifeStealItem : Item
{
    public override string GiveName() {
        return "Crimson Fangs";
    }

    public override string GiveDescription() {
        return "<color=lightblue>On hitting an enemy</color> have a <color=green>20%</color> <color=grey>(+10% each growth)</color> chance to recover <color=lightblue>10%</color> of your max health.";
    }

    public override void OnEnemyHit(PlayerStats __playerStats, int __stage, Hittable __hittable, List<Item> __itemChain) {
        if(Random.Range(0, 100) <= 10+(10*__stage)) __playerStats.GetPlayerHealth().Heal(__playerStats.GetPlayerHealth().GetMaxHealth()*0.1f, false);
    }
}

public class GamblingDiceItem : Item
{
    public override string GiveName() {
        return "Gambling Addict's Dice";
    }

    public override string GiveDescription() {
        return "<color=green>40%</color> <color=grey>(+10% each growth)</color> chance to double damage on the enemy, double instances of healing and half the damage taken, <color=lightblue>if the roll fails the damage and heal is halved and damage recieved is doubled</color>.";
    }

    public override void OnInflictDamage(PlayerStats __playerStats, int __stage, ref float __damage, List<Item> __itemChain) {
        if(Random.Range(0, 100) < 30+(10*__stage)) __damage *= 2f;
        else __damage /= 2f;
    }

    public override void OnPlayerDamaged(PlayerStats __playerStats, int __stage, ref float __damage, List<Item> __itemChain) {
        if(Random.Range(0, 100) < 30+(10*__stage)) __damage /= 2f;
        else __damage *= 2f;
    }

    public override void OnPlayerHealed(PlayerStats __playerStats, int __stage, ref float __health, List<Item> __itemChain) {
        if(Random.Range(0, 100) < 30+(10*__stage)) __health *= 2f;
        else __health /= 2f;
    }
}

public class ElectricAOEItem : Item
{
    public override string GiveName() {
        return "Over-Charged Battery";
    }

    public override string GiveDescription() {
        return "<color=lightblue>On hitting an enemy</color> have a <color=green>25%</color> <color=grey>(+10% each growth)</color> chance to spark <color=lightblue>3</color> bolts of electricity to nearby enemies that deals <color=green>50%</color> <color=grey>(+10% each growth)</color> of the damage that caused this item to trigger." ;
    }

    private float _dealtDamage;

    public override void OnEnemyHit(PlayerStats __playerStats, int __stage, Hittable __hittable, List<Item> __itemChain) {
        if(_dealtDamage <= 0) return; 
        if(Random.Range(0, 100) > 15+(10*__stage)) return;
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(__hittable.transform.position, 2f);
        List<Hittable> hittables = new List<Hittable>();
        foreach(Collider2D collider in nearbyColliders) {
            Hittable hittable = collider.GetComponent<Hittable>();
            if(hittable) {
                if(hittable != __hittable) {
                    hittables.Add(hittable);
                }
            }
        }
        if(hittables.Count == 0) return;
        __itemChain.Add(this);
        List<Vector3> posses = new List<Vector3>() {__hittable.transform.position};
        float totalDamage = 0;
        for(int i=0; i<3; i++) {
            if(hittables.Count == 0) break;
            Hittable hit = hittables[Random.Range(0, hittables.Count)];
            hittables.Remove(hit);
            posses.Add(hit.transform.position);
            float damage = _dealtDamage*(0.4f+(0.1f*__stage));
            __playerStats.OnInflictDamage(ref damage, __itemChain);
            __playerStats.OnEnemyHit(hit, __itemChain);
            totalDamage += damage;
            hit.TakeDamageWithColour(damage, __playerStats.GetPlayerXP(), "color=yellow");
            hit.Knockback((Vector2)(hit.transform.position-__hittable.transform.position).normalized, 3f, 0.5f*__stage);
        }
        _dealtDamage = 0;
        __playerStats.GetScreenShaker().ShakeRandom(totalDamage/10f, 50f, 0.1f);
        GameObject lineRendererPrefab = Resources.Load<GameObject>("ItemStuffs/SparkLine");
        LineRenderer sparkRenderer = GameObject.Instantiate(lineRendererPrefab, Vector2.zero, Quaternion.identity).GetComponent<LineRenderer>();
        sparkRenderer.SetPositions(posses.ToArray());
    }

    public override void OnInflictDamage(PlayerStats __playerStats, int __stage, ref float __damage, List<Item> __itemChain) {
        _dealtDamage = __damage;
    }
}

public class StandingGroundItem : Item
{
    public override string GiveName() {
        return "Stone Stance";
    }
    public override string GiveDescription() {
        return "<color=lightblue>Standing still</color> for <color=lightblue>0.5</color> seconds will cause you to take <color=lightblue>25%</color> less damage, deal <color=lightblue>200%</color> damage on player attacks and take <color=lightblue>no knockback from attacks.</color>";
    }

    private float _standingTimer = 0.5f;
    private ParticleSystem _reinforcedParticles;

    public override void Update(PlayerStats __playerStats, int __stage) {
        if(__playerStats.GetPlayerMovement().GetMovementVelocity() == Vector2.zero) {
            _standingTimer = Mathf.Max(_standingTimer-Time.deltaTime, -0.25f);
        } else {
            _standingTimer = Mathf.Min(_standingTimer+Time.deltaTime, 0.5f);
        }
        if(_reinforcedParticles) {
            if(_standingTimer <= 0 && !_reinforcedParticles.isPlaying) _reinforcedParticles.Play();
            else if(_standingTimer > 0) _reinforcedParticles.Stop();
        }
        else if(_standingTimer < 0) {
            _reinforcedParticles = GameObject.Instantiate(Resources.Load<GameObject>("ItemStuffs/StoneStanceParticles"), __playerStats.transform).GetComponent<ParticleSystem>();
        }
    }

    public override void OnPlayerDamaged(PlayerStats __playerStats, int __stage, ref float __damage, List<Item> __itemChain) {
        if(_standingTimer <= 0) __damage *= 0.85f-(0.1f*__stage);
    }

    public override void OnInflictDamage(PlayerStats __playerStats, int __stage, ref float __damage, List<Item> __itemChain) {
        if(_standingTimer <= 0 && __itemChain.Count==0) __damage *= 1.5f+(0.5f*__stage);
    }

    public override void OnPlayerKnockedback(PlayerStats __playerStats, int __stage, ref float _knockback, List<Item> __itemChain) {
        if(_standingTimer <= 0) _knockback = 0;
    }

    public override void OnPlayerHealed(PlayerStats __playerStats, int __stage, ref float __health, List<Item> __itemChain) {
        if(_standingTimer <= 0) __health *= 1.25f+(0.5f*__stage);
    }
}

public class PrimaryLongRangeItem : Item
{
    public override string GiveName() {
        return "Phantom Gloves";
    }

    public override string GiveDescription() {
        return "Using your <color=lightblue>primary ability</color> will cause a phantom fist to be launched into the enemy dealing <color=green>3</color> <color=grey>(+8 each growth)</color> damage to the enemy, piercing <color=lightblue>1</color> enemies.";
    }

    public override void OnPlayerPrimaryAttack(PlayerStats __playerStats, int __stage) {
        GameObject longRangePrefab = Resources.Load<GameObject>("ItemStuffs/LongRangeFist");
        FiredFist firedFist = GameObject.Instantiate(longRangePrefab, __playerStats.GetPlayerMovement().transform.position, Quaternion.identity).GetComponent<FiredFist>();
        List<Item> itemChain = new List<Item>() {this};
        firedFist.SetData(-5f+(8*__stage), 1, __playerStats.GetPlayerMovement().transform.right, __playerStats, itemChain, 5f);
    }

}

public class SecondaryLongRangeItem : Item {
    public override string GiveName() {
        return "Phantom Shoes";
    }

    public override string GiveDescription() {
        return "Using your <color=lightblue>secondary ability</color> will cause a phantom kick to be launched into the enemy dealing <color=green>10</color> <color=grey>(+12 each growth)</color> damage, piercing <color=lightblue>1</color> enemies.";
    }

    public override void OnPlayerSecondaryAttack(PlayerStats __playerStats, int __stage) {
        GameObject longRangePrefab = Resources.Load<GameObject>("ItemStuffs/LongRangeKick");
        FiredFist firedFist = GameObject.Instantiate(longRangePrefab, __playerStats.GetPlayerMovement().transform.position, Quaternion.identity).GetComponent<FiredFist>();
        List<Item> itemChain = new List<Item>() {this};
        firedFist.SetData(-2+(12*__stage), 1, __playerStats.GetPlayerMovement().transform.right, __playerStats, itemChain, 5f);
    }
}








