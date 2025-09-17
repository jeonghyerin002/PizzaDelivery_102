using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("카메라 설정")]
    public Transform target;                    // 따라다닐 플레이어
    public float distance = 5.0f;              // 플레이어와의 거리
    public float height = 2.0f;                // 높이 오프셋
    public float rotationSpeed = 2.0f;         // 회전 속도
    public float smoothSpeed = 10f;            // 부드러운 이동 속도

    [Header("마우스 감도")]
    [Range(0f, 100f)]
    public float mouseSensitivity = 100f;

    [Header("각도 제한")]
    public float minVerticalAngle = -30f;      // 최소 수직 각도
    public float maxVerticalAngle = 60f;       // 최대 수직 각도

    [Header("충돌 검사")]
    public LayerMask obstacleLayer = 1;        // 장애물 레이어
    public float minDistance = 1.5f;           // 최소 거리

    private float currentX = 0f;
    private float currentY = 0f;
    private float currentDistance;

    void Start()
    {
        // 마우스 커서 잠그기
        Cursor.lockState = CursorLockMode.Locked;

        // 타겟이 설정되지 않았으면 자동으로 찾기
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        currentDistance = distance;

        // 초기 각도 설정
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 마우스 입력
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        currentX += mouseX;
        currentY -= mouseY;

        // 수직 각도 제한
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        // 카메라 위치 계산
        Vector3 direction = new Vector3(0, 0, -currentDistance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 targetPosition = target.position + Vector3.up * height + rotation * direction;

        // 벽 충돌 검사
        CheckForObstacles(target.position + Vector3.up * height, targetPosition);

        // 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * height);
    }

    void CheckForObstacles(Vector3 targetPos, Vector3 cameraPos)
    {
        Vector3 direction = cameraPos - targetPos;
        float targetDistance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(targetPos, direction.normalized, out hit, targetDistance, obstacleLayer))
        {
            currentDistance = Mathf.Max(hit.distance - 0.5f, minDistance);
        }
        else
        {
            currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * 2f);
        }
    }
}