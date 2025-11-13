using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("참조")]
    public Camera mainCamera;
    public Transform cameraTransform;
    public Transform groundCheck;     // 지상 체크 기준점
    public LayerMask groundLayer;     // "Ground" 레이어
    public Animator animator;

    [Header("지상 움직임 설정")]
    public float moveSpeed = 10f;       // 지상 이동 속도
    public float walkSpeed = 10f;       //걷기 속도
    public float runSpeed = 20f;        //달리기 속도
    public float jumpForce = 8f;
    public float groundDistance = 0.4f; // 감지할 거리 (원의 반지름)
    public float rotationSpeed = 10f;
    public float breakForce = 7f;

    [HideInInspector]
    public bool isGrounded;

    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private bool wasGrounded;
    private Swinging swinging;

    [Header("Cinemachine")]
    public GameObject CinemachineCameraTarget;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    public float mouseSensitivity = 0.3f;

    public float BottomClamp = -30f;
    public float TopClamp = 70f;
    public float CameraAngleOverride = 0f;

    //감도 불러오기
    private const string SensitivityKey = "MouseSensitivity";
    private float defaultSensitivity = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        swinging = GetComponent<Swinging>();
        animator = GetComponentInChildren<Animator>();

        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;

        mouseSensitivity = PlayerPrefs.GetFloat(SensitivityKey, defaultSensitivity);

        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = groundCheckObj.transform;
        }
                
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        if (PauseMenu.isPaused) return;     //게임 일시정지 상태면 return

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _cinemachineTargetYaw += mouseX;
        _cinemachineTargetPitch -= mouseY;

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

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

    void Update()
    {
        if (PauseMenu.isPaused) return;     //게임 일시정지 상태면 return

        horizontalInput = Input.GetAxis("Horizontal"); // A, D
        verticalInput = Input.GetAxis("Vertical");     // W, S

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("Jump");
            Jump();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
    }

    void Jump()
    {
        // Y축 속도를 리셋해서 연속 점프 시 과속 방지
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // 위로 점프 힘 적용
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (PauseMenu.isPaused) return;     //게임 일시정지 상태면 return

        //지상 체크
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        //이동 처리
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * verticalInput + cameraRight * horizontalInput).normalized;

        animator.SetBool("isGrounded", isGrounded);

        float horizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        animator.SetFloat("moveSpeed", horizontalSpeed);

        // 지상일 때 이동
        if (isGrounded)
        {
            rb.drag = 1f;

            if(!wasGrounded)
            {
                animator.SetTrigger("Land");

                // 즉시 회전 리셋 및 고정
                rb.angularVelocity = Vector3.zero; // 남은 회전 제거
                var e = transform.eulerAngles;
                transform.rotation = Quaternion.Euler(0f, e.y, 0f); // 기울기 즉시 리셋
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

            rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
        }
        else
        {
            rb.drag = 0f;

            if (wasGrounded)
            {
                rb.angularDrag = 2f; // 공중 회전 저항
            }

            rb.constraints = RigidbodyConstraints.FreezeRotationY;
        }

        //스윙중이 아니고 플레이어가 회전한다면
        if (!swinging.isSwinging && moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }

        // 입력 크기
        float inputMag = Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput);

        //브레이크
        if (isGrounded && inputMag < 0.01f)
        {
            Vector3 v = rb.velocity;
            Vector3 hv = new Vector3(v.x, 0f, v.z);

            rb.AddForce(-hv * breakForce, ForceMode.Acceleration);  // 제동

            // 아주 느리면 정지
            if (hv.magnitude < 0.01)
                rb.velocity = new Vector3(0f, v.y, 0f);
        }

        wasGrounded = isGrounded;
    }

}
