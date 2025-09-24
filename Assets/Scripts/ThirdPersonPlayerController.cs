using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public float turnSmoothTime = 0.1f;        // 회전 부드러움

    [Header("땅 체크")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("카메라")]
    public Transform cameraTransform;          // 카메라 Transform (3인칭용)

    //옥상 배달 상호작용
    [Header("상호작용")]
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

        // 마우스 커서 잠그기
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Ground Check 오브젝트가 없으면 자동으로 생성
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = groundCheckObj.transform;
        }

        // 카메라가 설정되지 않았으면 메인 카메라 사용
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                cameraTransform = mainCam.transform;
        }
    }

    void Update()
    {
        // 땅 체크
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // 점프
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("점프");
        }

        //건물 옥상 배달 상호작용
        DeliveryZone();
    }

    void FixedUpdate()
    {
        // 이동 입력
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // 카메라 기준으로 이동 방향 계산 (3인칭)
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // 플레이어 회전
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // 이동 방향 계산
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // 이동 적용
            rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.Force);
        }

        // 수평 속도 제한 (Y축 속도는 유지)
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
       
        // 마우스 입력 받기 (구 Input Manager 방식)
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

    // 디버그용 기즈모
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }

    //건물 옥상 배달 상호작용 bool값
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("DeliveryBox"))
        {
            isDeliveryZone = true;
        }
    }

    //건물 옥상 배달 실제 상호작용
    void DeliveryZone()
    {
        if(isDeliveryZone)
        {
            Debug.Log("상호작용 할 수 있습니다.");    //이후에 인벤토리 구현 후 배달 완료 확인
            isDeliveryZone = false;
        }
    }
}