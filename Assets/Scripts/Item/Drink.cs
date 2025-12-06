using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : MonoBehaviour
{
    public float duration;
    private StaminaSystem staminaSystem;
    public float chaseSpeed = 3f;
    private Transform playerTarget;
    private void Start()
    {
        staminaSystem = GameObject.Find("Player").GetComponent<StaminaSystem>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }
    void Update()
    {
        if (playerTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, chaseSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어와 충돌");
            StartCoroutine(staminaSystem.UnlimitedStamina(duration));
            Destroy(gameObject);
        }
    }
}
