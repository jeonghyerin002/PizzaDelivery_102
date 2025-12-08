using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("참조")]
    public Camera mainCamera;
    public Transform cameraTransform;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Animator animator;
    public StaminaSystem staminaSystem;

    [Header("지상 움직임 설정")]
    public float moveSpeed = 10f;
    public float walkSpeed = 10f;
    public float runSpeed = 20f;
    public float jumpForce = 8f;
    public Vector3 groundBoxSize = new Vector3(0.4f, 0.1f, 0.4f);
    public float rotationSpeed = 10f;
    public float breakForce = 7f;
    public float airMultiplier = 0.5f;
    public float runStaminaCost = 15f;

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

    private const string SensitivityKey = "MouseSensitivity";
    private float defaultSensitivity = 0.3f;

    [HideInInspector] public static bool pausePlayer = false;

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
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        if (PauseMenu.isPaused) return;

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
        if (PauseMenu.isPaused) return;
        if (pausePlayer) return;

        if (PhoneOnOff.isPhone)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetTrigger("Jump");
            Jump();
        }

        bool isTryingToRun = Input.GetKey(KeyCode.LeftShift) && (horizontalInput != 0 || verticalInput != 0);

        if (isTryingToRun)
        {
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

        // 스윙 중 이동·회전·착지 처리 완전 비활성화
        if (swinging != null && swinging.isSwinging)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            animator.SetBool("isGrounded", true);
            wasGrounded = false;
            return;
        }

        isGrounded = Physics.CheckBox(groundCheck.position, groundBoxSize, transform.rotation, groundLayer);

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
        animator.SetFloat("fallSpeed", rb.velocity.y);

        if (isGrounded)
        {
            rb.drag = 1f;

            if (!wasGrounded)
            {
                float fallSpeed = rb.velocity.y;

                if (fallSpeed < hardLandVelocity || horizontalSpeed > hardLandMoveSpeed)
                {
                    animator.ResetTrigger("Jump");
                    animator.ResetTrigger("Land");
                    animator.SetTrigger("HardLand");
                }
                else
                {
                    animator.ResetTrigger("Jump");
                    animator.ResetTrigger("HardLand");
                    animator.SetTrigger("Land");
                }

                rb.angularVelocity = Vector3.zero;
                var e = transform.eulerAngles;
                transform.rotation = Quaternion.Euler(0f, e.y, 0f);
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
            rb.constraints = RigidbodyConstraints.FreezeRotationY;

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

        float inputMag = Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput);

        if (isGrounded && inputMag < 0.01f)
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
