using UnityEngine;

public interface EnemyAI {
    public void SetData(Transform __player, ScreenShaker __screenShaker);
    public void Stun(float __duration);
}