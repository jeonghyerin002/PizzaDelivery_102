using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerController : MonoBehaviour
{
    [Header("�̵� ����")]
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public float turnSmoothTime = 0.1f;

    [Header("�� üũ")]
    public Transform groundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("ī�޶� ����")]
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
