using UnityEngine;
using System.Collections;

public class DeliveryDrone : MonoBehaviour
{
    [Header("설정")]
    public float approachSpeed = 20f;   // 진입 속도
    public float exitSpeed = 30f;       // 퇴장 속도
    public float dropHeight = 2f;       // 드랍 높이

    private Transform player;
    private GameObject itemPrefab;

    public void StartDelivery(GameObject item, Transform playerTransform)
    {
        itemPrefab = item;
        player = playerTransform;
        StartCoroutine(DeliverySequence());
    }

    IEnumerator DeliverySequence()
    {
        //목표 지점
        Vector3 dropPoint = player.position + Vector3.up * dropHeight;

        // 접근
        while (Vector3.Distance(transform.position, dropPoint) > 0.5f)
        {
            // 플레이어 머리위 추적
            dropPoint = player.position + Vector3.up * dropHeight;

            transform.position = Vector3.MoveTowards(transform.position, dropPoint, approachSpeed * Time.deltaTime);
            transform.LookAt(dropPoint);
            yield return null;
        }

        // 드랍
        Instantiate(itemPrefab, transform.position, Quaternion.identity);

        // 퇴장
        Vector3 exitDirection = (transform.forward + Vector3.up).normalized;

        float timer = 0f;
        while (timer < 2f) // 2초동안 퇴장 비행
        {
            transform.position += exitDirection * exitSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(exitDirection), Time.deltaTime * 5f);

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}