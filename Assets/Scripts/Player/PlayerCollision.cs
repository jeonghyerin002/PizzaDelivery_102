using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [Header("내구도 설정")]
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }

    [Header("충격 민감도")]
    public float damageThreshold = 10f; // 데미지를 입기 시작하는 최소 속도

    [Tooltip("벽에 부딪칠 때(수평) 데미지 계수")]
    public float horizontalMultiplier = 2.0f; // 벽 충돌은 아프게!

    [Tooltip("바닥에 떨어질 때(수직) 데미지 계수")]
    public float verticalMultiplier = 0.5f;   //낙하 데미지는 약하게 (0.5배)

    public float damageCooltime = 1f;
    private float lastDamageTime;
    private bool isBroken = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time < lastDamageTime + damageCooltime)
        {
            Debug.Log("무적시간");
            return;
        }
        Vector3 relativeVel = collision.relativeVelocity;

        // 2. 수직(Y) 속도와 수평(X, Z) 속도 분리 계산
        float vertSpeed = Mathf.Abs(relativeVel.y); // 위아래 속도 (절대값)
        float horizSpeed = new Vector3(relativeVel.x, 0, relativeVel.z).magnitude; // 수평 속도

        float totalDamage = 0f;

        // 3. 수직 충격 (낙하) 계산 - verticalMultiplier 적용
        if (vertSpeed > damageThreshold)
        {
            totalDamage += (vertSpeed - damageThreshold) * verticalMultiplier;
        }

        // 4. 수평 충격 (벽 충돌) 계산 - horizontalMultiplier 적용
        if (horizSpeed > damageThreshold)
        {
            totalDamage += (horizSpeed - damageThreshold) * horizontalMultiplier;
        }

        // 5. 데미지가 발생했다면 적용
        if (totalDamage > 0)
        {
            QuestManager.instance.PackageDamage(totalDamage);
            Debug.Log($"충돌! {collision.gameObject.name}에 수직속도:{vertSpeed:F1}, 수평속도:{horizSpeed:F1} => 총 데미지:{totalDamage:F1}");
        }
    }
}
