using UnityEngine;

public class ImprovedGrapplingSystem : MonoBehaviour
{
    public Transform playerTransform;
    public Transform ropeTip;
    public Camera playerCamera;

    public LayerMask grapplingLayer;
    public float maxGrappleDistance = 50f;
    public float upwardAngle = 15f;

    public float ropeThickness = 0.1f;
    public float spring = 4.5f;
    public float damper = 7f;
    public float massScale = 1f;
    public float pullForce = 2000f;
    public float pullSpeed = 20f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.5f;

    private SpringJoint joint;
    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    private bool isGrappling = false;
    private Rigidbody playerRb;

    public bool IsGrappling()
    {
        return isGrappling;
    }

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = ropeThickness;
        lineRenderer.endWidth = ropeThickness;
        lineRenderer.enabled = false;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        playerRb = playerTransform.GetComponent<Rigidbody>();
        if (playerRb == null)
        {
            Debug.LogError("Player needs Rigidbody component!");
        }
    }

    void Update()
    {
        if (PauseMenu.isPaused) return;      //게임 일시정지 상태면 return

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked!");
            StartGrapple();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (Input.GetMouseButton(1) && isGrappling)
        {
            PullTowardsGrapplePoint();
        }

        if (isGrappling)
        {
            DrawRope();
            CheckGroundAndStabilize();
        }
    }

    void StartGrapple()
    {
        Debug.Log("StartGrapple Called!");

        if (playerCamera == null)
        {
            Debug.LogError("Camera is NULL!");
            return;
        }

        Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Player needs Rigidbody!");
            return;
        }

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        Vector3 direction = ray.direction;
        direction = Quaternion.AngleAxis(-upwardAngle, playerCamera.transform.right) * direction;

        Debug.Log("Shooting raycast from: " + ray.origin + " in direction: " + direction);
        Debug.DrawRay(ray.origin, direction * maxGrappleDistance, Color.red, 2f);

        RaycastHit hit;

        if (Physics.Raycast(ray.origin, direction, out hit, maxGrappleDistance, grapplingLayer))
        {
            Debug.Log("HIT! Object: " + hit.collider.name + " Layer: " + hit.collider.gameObject.layer);
            grapplePoint = hit.point;

            joint = playerTransform.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(playerTransform.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

            lineRenderer.enabled = true;
            isGrappling = true;

            Debug.Log("Grapple Success: " + hit.collider.name);
        }
        else
        {
            Debug.Log("MISS! No grapple point found. Layer mask: " + grapplingLayer.value);
        }
    }

    void StopGrapple()
    {
        lineRenderer.enabled = false;
        isGrappling = false;

        if (joint != null)
        {
            Destroy(joint);
        }

    }

    void DrawRope()
    {
        if (!joint) return;

        lineRenderer.SetPosition(0, ropeTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    void PullTowardsGrapplePoint()
    {
        if (playerRb == null || !isGrappling) return;

        Vector3 direction = (grapplePoint - playerTransform.position).normalized;
        playerRb.AddForce(direction * pullForce * Time.deltaTime);

        Vector3 currentVelocity = playerRb.velocity;
        Vector3 targetVelocity = direction * pullSpeed;
        playerRb.velocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * 2f);
    }

    void CheckGroundAndStabilize()
    {
        if (playerRb == null) return;

        bool isGrounded = Physics.Raycast(playerTransform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            Vector3 velocity = playerRb.velocity;
            velocity.x *= 0.9f;
            velocity.z *= 0.9f;
            playerRb.velocity = velocity;
        }
    }
}