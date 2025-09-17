using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("ī�޶� ����")]
    public Transform target;                    // ����ٴ� �÷��̾�
    public float distance = 5.0f;              // �÷��̾���� �Ÿ�
    public float height = 2.0f;                // ���� ������
    public float rotationSpeed = 2.0f;         // ȸ�� �ӵ�
    public float smoothSpeed = 10f;            // �ε巯�� �̵� �ӵ�

    [Header("���콺 ����")]
    [Range(0f, 100f)]
    public float mouseSensitivity = 100f;

    [Header("���� ����")]
    public float minVerticalAngle = -30f;      // �ּ� ���� ����
    public float maxVerticalAngle = 60f;       // �ִ� ���� ����

    [Header("�浹 �˻�")]
    public LayerMask obstacleLayer = 1;        // ��ֹ� ���̾�
    public float minDistance = 1.5f;           // �ּ� �Ÿ�

    private float currentX = 0f;
    private float currentY = 0f;
    private float currentDistance;

    void Start()
    {
        // ���콺 Ŀ�� ��ױ�
        Cursor.lockState = CursorLockMode.Locked;

        // Ÿ���� �������� �ʾ����� �ڵ����� ã��
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        currentDistance = distance;

        // �ʱ� ���� ����
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ���콺 �Է�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        currentX += mouseX;
        currentY -= mouseY;

        // ���� ���� ����
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        // ī�޶� ��ġ ���
        Vector3 direction = new Vector3(0, 0, -currentDistance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 targetPosition = target.position + Vector3.up * height + rotation * direction;

        // �� �浹 �˻�
        CheckForObstacles(target.position + Vector3.up * height, targetPosition);

        // �ε巴�� �̵�
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