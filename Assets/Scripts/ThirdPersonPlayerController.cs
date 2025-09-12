using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerController : MonoBehaviour
{
    [Header("이동 조작")]
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public float turnSmoothTime = 0.1f;

    [Header("땅 체크")]
    public Transform groundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("카메라 참조")]
    public Transform cameraTransform;

    public Rigidbody rb;
    public bool isGrounded;

    private float turnSmoothVelocity;

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
