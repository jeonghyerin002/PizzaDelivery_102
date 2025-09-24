using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerController : MonoBehaviour
{
    [Header("�̵� ����")]
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public float turnSmoothTime = 0.1f;        // ȸ�� �ε巯��

    [Header("�� üũ")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("ī�޶�")]
    public Transform cameraTransform;          // ī�޶� Transform (3��Ī��)

    //���� ��� ��ȣ�ۿ�
    [Header("��ȣ�ۿ�")]
    public GameObject deliveryZone;
    public bool isDeliveryZone = false;

    private Rigidbody rb;
    private bool isGrounded;
    private float turnSmoothVelocity;

    [Header("Cinemachine")]
    public GameObject CinemachineCameraTarget;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    public float mouseSensitivity = 1.0f;

    public float BottomClamp = -30f;
    public float TopClamp = 70f;
    public float CameraAngleOverride = 0f;



    void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        // ���콺 Ŀ�� ��ױ�
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Ground Check ������Ʈ�� ������ �ڵ����� ����
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = groundCheckObj.transform;
        }

        // ī�޶� �������� �ʾ����� ���� ī�޶� ���
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                cameraTransform = mainCam.transform;
        }
    }

    void Update()
    {
        // �� üũ
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // ����
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("����");
        }

        //�ǹ� ���� ��� ��ȣ�ۿ�
        DeliveryZone();
    }

    void FixedUpdate()
    {
        // �̵� �Է�
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // ī�޶� �������� �̵� ���� ��� (3��Ī)
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // �÷��̾� ȸ��
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // �̵� ���� ���
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // �̵� ����
            rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.Force);
        }

        // ���� �ӵ� ���� (Y�� �ӵ��� ����)
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalVelocity.magnitude > moveSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
       
        // ���콺 �Է� �ޱ� (�� Input Manager ���)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _cinemachineTargetYaw += mouseX;
        _cinemachineTargetPitch -= mouseY;

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(
            _cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw,
            0.0f
        );
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    // ����׿� �����
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }

    //�ǹ� ���� ��� ��ȣ�ۿ� bool��
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("DeliveryBox"))
        {
            isDeliveryZone = true;
        }
    }

    //�ǹ� ���� ��� ���� ��ȣ�ۿ�
    void DeliveryZone()
    {
        if(isDeliveryZone)
        {
            Debug.Log("��ȣ�ۿ� �� �� �ֽ��ϴ�.");    //���Ŀ� �κ��丮 ���� �� ��� �Ϸ� Ȯ��
            isDeliveryZone = false;
        }
    }
}