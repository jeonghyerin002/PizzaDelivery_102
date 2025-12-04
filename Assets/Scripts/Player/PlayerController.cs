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
    public StaminaSystem staminaSystem;

    [Header("지상 움직임 설정")]
    public float moveSpeed = 10f;       // 지상 이동 속도
    public float walkSpeed = 10f;       //걷기 속도
    public float runSpeed = 20f;        //달리기 속도
    public float jumpForce = 8f;
    public Vector3 groundBoxSize = new Vector3(0.4f, 0.1f, 0.4f);
    public float rotationSpeed = 10f;
    public float breakForce = 7f;
    public float runStaminaCost = 15f;   //추가 달리기 스테미나 초당 소모량

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

    [Header("착지 설정")]
    public float hardLandVelocity = -20f;
    public float hardLandMoveSpeed = 20f;

    //감도 불러오기
    private const string SensitivityKey = "MouseSensitivity";
    private float defaultSensitivity = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        swinging = GetComponent<Swinging>();
        animator = GetComponentInChildren<Animator>();
        if (staminaSystem == null) staminaSystem = GetComponent<StaminaSystem>();

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
        AudioManager.instance.PlayBGM("main");
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        if (PauseMenu.isPaused) return;     //게임 일시정지 상태면 return
        float mouseX = 0f;
        float mouseY = 0f;

        if (!PhoneOnOff.isPhone)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        }

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
        if (PhoneOnOff.isPhone)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }
        horizontalInput = Input.GetAxis("Horizontal"); // A, D
        verticalInput = Input.GetAxis("Vertical");     // W, S


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("Jump");
            Jump();
        }

        bool isTryingToRun = Input.GetKey(KeyCode.LeftShift) && (horizontalInput != 0 || verticalInput != 0);

        if (isTryingToRun)
        {
            // 스테미나를 소모 시도 (성공하면 true, 실패하면 false)
            // Time.deltaTime을 곱해서 초당 소모량으로 계산
            if (staminaSystem.DrainStamina(runStaminaCost * Time.deltaTime))
            {
                moveSpeed = runSpeed;
            }
            else
            {
                moveSpeed = walkSpeed;
            }
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
        isGrounded = Physics.CheckBox(groundCheck.position, groundBoxSize, transform.rotation, groundLayer);

        //이동 처리
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * verticalInput + cameraRight * horizontalInput).normalized;

        if (swinging.isSwinging)
        {
            animator.SetBool("isGrounded", true);
        }
        else
        {
            animator.SetBool("isGrounded", isGrounded);
        }

        float horizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        animator.SetFloat("moveSpeed", horizontalSpeed);
        animator.ResetTrigger("Jump");
        animator.SetFloat("fallSpeed", rb.velocity.y);



        // 지상일 때 이동
        if (isGrounded)
        {
            rb.drag = 1f;

            if (!wasGrounded)
            {
                float fallSpeed = rb.velocity.y;

                if (fallSpeed < hardLandVelocity || horizontalSpeed > hardLandMoveSpeed)
                {
                    animator.ResetTrigger("Land");
                    animator.SetTrigger("HardLand");
                    Debug.Log($"강한 착지 속도 {fallSpeed}");
                }
                else
                {
                    animator.ResetTrigger("HardLand");
                    animator.SetTrigger("Land");
                }

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

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(groundCheck.position, transform.rotation, transform.localScale);
            Gizmos.matrix = rotationMatrix;

            Gizmos.DrawWireCube(Vector3.zero, groundBoxSize * 2);
        }
    }

}
