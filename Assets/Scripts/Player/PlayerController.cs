using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("참조")]
    public Camera mainCamera;
    public Transform cameraTransform;
    public Transform groundCheck;
    public LayerMask groundLayer;       //땅 레이어
    public Animator animator;
    public StaminaSystem staminaSystem;

    [Header("지상 움직임 설정")]
    public float moveSpeed = 10f;       //이동 속도
    public float walkSpeed = 10f;       //걷는 속도
    public float runSpeed = 20f;        //달리기 속도
    public float jumpForce = 8f;        //점프 힘
    public Vector3 groundBoxSize = new Vector3(0.4f, 0.1f, 0.4f);
    public float rotationSpeed = 10f;
    public float breakForce = 7f;
    public float airMultiplier = 0.5f;
    public float runStaminaCost = 15f;      //달리기 스테미나 초당 소모량

    [HideInInspector] public bool isGrounded;

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

    public bool pausePlayer = false;

    private const string SensitivityKey = "MouseSensitivity";
    private float defaultSensitivity = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        swinging = GetComponent<Swinging>();
        //animator = GetComponentInChildren<Animator>();
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
        if (pausePlayer)
        {
            return;
        }

        if (PhoneOnOff.isPhone)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        horizontalInput = Input.GetAxis("Horizontal");      //wasd 입력
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("Jump");
            Jump();
        }

        bool isTryingToRun = Input.GetKey(KeyCode.LeftShift) && (horizontalInput != 0 || verticalInput != 0);

        if (isTryingToRun)
        {
            //스테미나 소모 시도
            //Time.dletaTime을 곱해서 초당 소모량으로 계산
            if (staminaSystem.DrainStamina(runStaminaCost * Time.deltaTime))
                moveSpeed = runSpeed;
            else
                moveSpeed = walkSpeed;

        }
        else moveSpeed = walkSpeed;
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (PauseMenu.isPaused) return;
        if (pausePlayer) return;

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
        animator.SetFloat("fallSpeed", rb.velocity.y);

        if (isGrounded && !swinging.isSwinging)
        {
            rb.drag = 1f;

            if (!wasGrounded)
            {
                float fallSpeed = rb.velocity.y;

                if (fallSpeed < hardLandVelocity || horizontalSpeed > hardLandMoveSpeed)
                {
                    animator.ResetTrigger("Jump");
                    animator.ResetTrigger("Land");              //다른 애니메이션 리셋
                    animator.SetTrigger("HardLand");            //구르기 애니메이션
                }
                else
                {
                    animator.ResetTrigger("Jump");
                    animator.ResetTrigger("HardLand");
                    animator.SetTrigger("Land");
                }

                rb.angularVelocity = Vector3.zero;      //남은 회전 제거
                var e = transform.eulerAngles;
                transform.rotation = Quaternion.Euler(0f, e.y, 0f);     //기울기 리셋
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

            rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }
        }
        else
        {
            rb.drag = 0.3f;

            if (wasGrounded)
            {
                rb.angularDrag = 2f;        //공중 회전 저항
            }

            rb.constraints = RigidbodyConstraints.FreezeRotationY;

            if (!swinging.isSwinging)
            {
                Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                if (flatVel.magnitude < 7)
                {
                    rb.AddForce(moveDirection * moveSpeed * airMultiplier, ForceMode.Acceleration);
                }

                if (moveDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
                }
            }
        }

        float inputMag = Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput);     //입력 크기

        if (isGrounded && inputMag < 0.01f)     //브레이크
        {
            Vector3 v = rb.velocity;
            Vector3 hv = new Vector3(v.x, 0f, v.z);

            rb.AddForce(-hv * breakForce, ForceMode.Acceleration);

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
