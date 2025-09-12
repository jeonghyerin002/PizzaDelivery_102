using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Ÿ��")]
    public Transform player;
    public Transform cameraTarget;

    [Header("ī�޶� ����")]
    public float distance = 5f;
    public float height = 2.0f;
    public float sensitivity = 2.0f;
    public float shoulderOffset = 0.8f;

    [Header("ī�޶� ����")]
    public float minY = -40f;
    public float maxY = 70f;

    [Header("�浹 ����")]
    public LayerMask collisionMask = -1;
    public float minDistance = 1f;

    [Header("�ε巯��")]
    public float positionSmoothTime = 0.15f;
    public float rotationSmoothTime = 0.05f;
    public float heightSmoothTime = 0.2f;

    private float currentX = 0f;
    private float currentY = 0f;
    private Vector3 positionVelocity;
    private Vector3 currentRotationVelocity;
    private float smoothedTargetY = 0f;
    private float targetYVelocity = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        currentX = player.eulerAngles.y;
        currentY = 0;
        smoothedTargetY = cameraTarget.position.y;

        if(cameraTarget == null)
        {
            GameObject cameraRoot = new GameObject("PlayerCameraRoot");
            cameraRoot.transform.SetParent(player);
            cameraRoot.transform.localPosition = new Vector3(0f, 1.375f, 0f);
            cameraTarget = cameraRoot.transform;
        }

    }


    void Update()
    {
        
    }

    public Vector3 GetCrosshairWorldPosition(float maxDistance = 100f)
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            return hit.point;
        }

        return ray.origin + ray.direction * maxDistance;
    }

    void HandleInput()
    {
        float mouseX = Input.GetAxis("MouseX") * sensitivity;
        float mouseY = Input.GetAxis("MouseY") * sensitivity;

        currentX += mouseX;
        currentY -= mouseY;
        currentY = Mathf.Clamp(currentY, minY, maxY);

        //���콺 �ٷ� �Ÿ� ����
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if( (Mathf.Abs(scroll) > 0.1f))
        {
            distance = Mathf.Clamp(distance - scroll * 2.0f, 2.0f, 2.0f);
        }
    }
    void PositionCamera()
    {
        Vector3 targetPos = cameraTarget.position;

        smoothedTargetY = Mathf.SmoothDamp(smoothedTargetY, targetPos.y, ref targetYVelocity, heightSmoothTime);
        Vector3 smoothedTargetPos = new Vector3(targetPos.x, smoothedTargetY, targetPos.z);

        Vector3 targetEuler = new Vector3(currentY, currentX, 0f);

        Vector3 currentEuler = new Vector3(
            Mathf.SmoothDampAngle(transform.eulerAngles.x, targetEuler.x, ref currentRotationVelocity.x, rotationSmoothTime),
            Mathf. SmoothDampAngle(transform.eulerAngles.y, targetEuler.y, ref currentRotationVelocity.y, rotationSmoothTime),
            0f
      );

        Quaternion smoothRotation = Quaternion.Euler(currentEuler);

        //��� �ʸ� ������ ��� (�ε巯�� Ÿ�� ��ġ ���)
        Vector3 offset = new Vector3(shoulderOffset, height, -distance);
        Vector3 desiredPosition = smoothedTargetPos + smoothRotation * offset;

        //�� �浹 üũ(���� ī�� ��ġ ����)
        Vector3 direction = (desiredPosition - targetPos).normalized;
        float distanceToTarget = Vector3.Distance(targetPos, desiredPosition);

        RaycastHit hit;
        if (Physics.Raycast(targetPos, direction, out hit, distanceToTarget, collisionMask))
        {

            //�浹 �� ��ġ ����
            desiredPosition = hit.point - direction * 0.2f;

            //�ּ� �Ÿ� ����
            if (Vector3.Distance(targetPos, desiredPosition) < minDistance)
            {
                desiredPosition = targetPos + direction * minDistance;
            }
        }

        //ī�޶� ��ġ �ε巴�� �̵�
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref positionVelocity, positionSmoothTime);

        //ī�޶� ȸ�� ����
        transform.rotation = smoothRotation;
    }

    void LateUpdate()
    {
        HandleInput();
        PositionCamera();
    }
}
