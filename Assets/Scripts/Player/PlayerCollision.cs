using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [Header("충돌 설정")]
    public float minImpactForce = 5f;
    public float buildingDamage = 2f;
    public float groundDamage = 2f;

    public float damageCooltime = 1f;
    private float lastDamageTime;
    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time < lastDamageTime + damageCooltime)
        {
            Debug.Log("무적시간");
            return;
        }
        Debug.Log($"[충돌] 부딪힌 물체: {collision.gameObject.name} / 태그: {collision.gameObject.tag}");
        float impactForce = collision.relativeVelocity.magnitude;

        //너무 살살 부딪힌 건 무시
        if (impactForce < minImpactForce) return;

        float finalDamage = 0f;

        //부딫힌 대상 확인
        if (collision.gameObject.CompareTag("Building")) // 건물 태그 확인
        {
            finalDamage = buildingDamage * (impactForce / 10f);
            Debug.Log($"건물 충돌! 강도: {impactForce:F1}, 데미지: {finalDamage:F1}");
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            finalDamage = groundDamage;
        }
        else
        {
            finalDamage = 5f;
        }

        if (finalDamage > 0)
        {
            QuestManager.instance.PackageDamage(finalDamage);

            lastDamageTime = Time.time;
        }
    }
}
