using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRopeSystem : MonoBehaviour
{
    [Header("���̾��Ű ����")]
    public Transform playerTransform;          // ���� �÷��̾� Transform (���� �Ҵ�)
    public Transform ropeTip;                  // ������ ������ ��ġ (��, ���� ��)

    [Header("���� ����")]
    public LayerMask grapplingLayer = 1;       // �׷��� ������ ������Ʈ ���̾�
    public float maxGrappleDistance = 20f;     // �ִ� �׷��� �Ÿ�
    public float detectionRadius = 15f;        // Ž�� �ݰ�
    public float ropeThickness = 0.1f;         // ���� �β�

    [Header("���� ����")]
    public float spring = 4.5f;
    public float damper = 7.0f;
    public float massScale = 1.0f;
    public float ropeMinDistance = 0.25f;
    public float pullForce = 1000f;            // �÷��̾� ���� ��
    public float pullSpeed = 20f;              // �÷��̾� ���� �ִ� �ӵ�
    public float swingForce = 500f;            // ���� ���� ��

    [Header("��ü ������� ����")]
    public float objectPullForce = 800f;       // ��ü�� ������� ��
    public float objectPullRange = 15f;        // ��ü ������� �ִ� �Ÿ�
    public LayerMask pullableObjects = 1;      // ����� �� �ִ� ��ü ���̾�

    [Header("UI ����")]
    public Material ropeMaterial;              // ���� ����
    public Color ropeColor = Color.white;      // �Ϲ� ���� ����
    public Color objectPullColor = Color.cyan; // ��ü ������� ���� ����
    public Color targetIndicatorColor = Color.red; // Ÿ�� ǥ�� ����

    // ������Ʈ
    private LineRenderer lineRenderer;
    private SpringJoint springJoint;
    private Rigidbody playerRigidbody;
    private Camera playerCamera;

    // ���� ����
    private bool isGrappling = false;
    private Vector3 grapplePoint;
    private GameObject currentTarget;
    private List<GameObject> nearbyTargets = new List<GameObject>();
    private Transform bestTarget;

    // ��ü ������� ����
    private GameObject targetObject;
    private SpringJoint objectSpringJoint;
    private bool isPullingObject = false;

    void Start()
    {
        // �÷��̾� Transform�� �������� �ʾ����� �ڵ����� ã��
        if (playerTransform == null)
        {
            playerTransform = GetPlayerTransform();
        }

        // ���� Tip�� �������� �ʾ����� �ڵ����� ����
        if (ropeTip == null)
        {
            CreateRopeTip();
        }

        if (playerTransform != null)
        {
            playerRigidbody = playerTransform.GetComponent<Rigidbody>();
        }

        playerCamera = Camera.main;

        // LineRenderer ����
        SetupLineRenderer();

        // �ֱ������� �ֺ� Ÿ�� �˻�
        InvokeRepeating("FindNearbyTargets", 0f, 0.5f);
    }

    void SetupLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        if (ropeMaterial == null)
            ropeMaterial = new Material(Shader.Find("Sprites/Default"));

        lineRenderer.material = ropeMaterial;
        lineRenderer.startWidth = ropeThickness;
        lineRenderer.endWidth = ropeThickness;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        // �ֺ� Ÿ�� ã�� �� ���� Ÿ�� ����
        FindBestTarget();

        // ���� ���콺�� ���� �߻�/����
        if (Input.GetMouseButtonDown(0))
        {
            if (!isGrappling)
                StartAutoGrapple();
            else
                StopGrapple();
        }

        // ������ ���콺�� ��ü �������
        if (Input.GetMouseButtonDown(1))
        {
            if (!isPullingObject)
                StartObjectPull();
            else
                StopObjectPull();
        }

        // �׷��ø� ���� �� �߰� ����
        if (isGrappling)
        {
            UpdateRope();

            // Shift�� ����
            if (Input.GetKey(KeyCode.LeftShift))
                PullTowardsGrapplePoint();

            // A, D�� ���� ����
            HandleSwingInput();
        }

        // ��ü ������� ���� ��
        if (isPullingObject)
        {
            UpdateObjectPull();
        }
    }

    void FindNearbyTargets()
    {
        nearbyTargets.Clear();

        Vector3 searchPosition = GetRopeStartPosition();
        Collider[] colliders = Physics.OverlapSphere(searchPosition, detectionRadius, grapplingLayer);

        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject && (playerTransform == null || col.gameObject != playerTransform.gameObject))
            {
                nearbyTargets.Add(col.gameObject);
            }
        }
    }

    void FindBestTarget()
    {
        if (nearbyTargets.Count == 0 || playerTransform == null)
        {
            bestTarget = null;
            return;
        }

        Transform closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 ropeStartPos = GetRopeStartPosition();
        Vector3 cameraForward = playerCamera.transform.forward;

        foreach (GameObject target in nearbyTargets)
        {
            if (target == null) continue;

            Vector3 directionToTarget = target.transform.position - ropeStartPos;
            float distance = directionToTarget.magnitude;

            // �Ÿ� ���� Ȯ��
            if (distance > maxGrappleDistance) continue;

            // ī�޶� ������� ���� ���
            float angle = Vector3.Angle(cameraForward, directionToTarget.normalized);
            if (angle > 90f) continue;

            // �÷��̾�� ���� �ִ� Ÿ�� �켱
            float heightDifference = target.transform.position.y - ropeStartPos.y;
            if (heightDifference < -2f) continue;

            // ��ֹ� �˻�
            RaycastHit hit;
            if (Physics.Raycast(ropeStartPos, directionToTarget.normalized, out hit, distance, grapplingLayer))
            {
                if (hit.collider.gameObject == target)
                {
                    // �Ÿ��� ������ ������ ���� ���
                    float score = distance - (heightDifference * 0.5f) + (angle * 0.1f);

                    if (score < closestDistance)
                    {
                        closestDistance = score;
                        closest = target.transform;
                    }
                }
            }
        }

        bestTarget = closest;
    }

    void StartAutoGrapple()
    {
        if (bestTarget == null) return;

        grapplePoint = bestTarget.position;
        isGrappling = true;
        currentTarget = bestTarget.gameObject;

        // ���� ���� �׸��� ����
        lineRenderer.positionCount = 2;

        // �÷��̾��� Rigidbody�� SpringJoint ����
        if (playerTransform != null && playerRigidbody != null)
        {
            springJoint = playerTransform.gameObject.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = grapplePoint;

            // �Ÿ� ��� �� SpringJoint ����
            float distance = Vector3.Distance(playerTransform.position, grapplePoint);
            springJoint.maxDistance = distance * 0.8f;
            springJoint.minDistance = distance * ropeMinDistance;
            springJoint.spring = spring;
            springJoint.damper = damper;
            springJoint.massScale = massScale;

            Debug.Log("���� �߻�! Ÿ��: " + currentTarget.name + ", �Ÿ�: " + distance.ToString("F1") + "m");
        }
    }

    void UpdateRope()
    {
        if (lineRenderer.positionCount == 2)
        {
            lineRenderer.SetPosition(0, GetRopeStartPosition());
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }

    void PullTowardsGrapplePoint()
    {
        if (!isGrappling || playerRigidbody == null || playerTransform == null) return;

        Vector3 directionToGrapple = (grapplePoint - playerTransform.position).normalized;
        float currentVelocity = Vector3.Dot(playerRigidbody.velocity, directionToGrapple);

        if (currentVelocity < pullSpeed)
        {
            Vector3 pullForceVector = directionToGrapple * pullForce * Time.deltaTime;
            playerRigidbody.AddForce(pullForceVector, ForceMode.Force);
        }
    }

    void HandleSwingInput()
    {
        if (playerTransform == null) return;

        float horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            Vector3 ropeDirection = (grapplePoint - playerTransform.position).normalized;
            Vector3 swingDirection = Vector3.Cross(ropeDirection, Vector3.up).normalized;

            Vector3 swingForceVector = swingDirection * horizontal * swingForce * Time.deltaTime;
            playerRigidbody.AddForce(swingForceVector, ForceMode.Force);
        }
    }

    void StopGrapple()
    {
        if (!isGrappling) return;

        isGrappling = false;
        lineRenderer.positionCount = 0;
        currentTarget = null;

        if (springJoint != null)
        {
            Destroy(springJoint);
            springJoint = null;
        }

        Debug.Log("���� ����!");
    }

    void StartObjectPull()
    {
        // Tip ��ġ���� ���콺 �������� ����ĳ��Ʈ
        Vector3 tipPosition = GetRopeStartPosition();
        Ray mouseRay = playerCamera.ScreenPointToRay(Input.mousePosition);

        // Tip���� ���콺 �������� ���ϴ� ���� ���
        Vector3 direction = (mouseRay.GetPoint(100f) - tipPosition).normalized;

        Debug.Log("����ĳ��Ʈ ���� ��ġ: " + tipPosition);
        Debug.Log("����ĳ��Ʈ ����: " + direction);
        Debug.Log("����ĳ��Ʈ ����: " + objectPullRange);
        Debug.Log("Ÿ�� ���̾�: " + pullableObjects.value);

        RaycastHit hit;
        // ���̾� ����ũ�� Physics.Raycast�� ���� ����
        if (Physics.Raycast(tipPosition, direction, out hit, objectPullRange, pullableObjects))
        {
            Debug.Log("����ĳ��Ʈ hit! ������Ʈ: " + hit.collider.name + ", ���̾�: " + hit.collider.gameObject.layer);

            // �÷��̾� �ڽ��� ����
            if (hit.collider.gameObject == playerTransform.gameObject)
            {
                Debug.Log("�÷��̾� �ڽ��� hit����");
                return;
            }

            // ��ü�� Rigidbody�� �ִ��� Ȯ��
            Rigidbody objectRb = hit.collider.GetComponent<Rigidbody>();
            if (objectRb != null)
            {
                targetObject = hit.collider.gameObject;
                isPullingObject = true;

                // ��ü�� SpringJoint �߰�
                objectSpringJoint = targetObject.AddComponent<SpringJoint>();
                objectSpringJoint.autoConfigureConnectedAnchor = false;
                objectSpringJoint.connectedAnchor = tipPosition;

                // SpringJoint ���� (������ �κ�)
                float distance = Vector3.Distance(targetObject.transform.position, tipPosition);
                objectSpringJoint.maxDistance = distance * 0.8f;    // 0.1f �� 0.8f�� ����
                objectSpringJoint.minDistance = 1f;                 // 0f �� 1f�� ����
                objectSpringJoint.spring = 200f;                    // objectPullForce(800f) �� 200f�� ����
                objectSpringJoint.damper = 150f;                    // 50f �� 150f�� ����
                objectSpringJoint.massScale = 0.1f;                 // 1f �� 0.1f�� ����

                // ��ü�� �巡�� ���� (���û��� - �� �ε巴��)
                objectRb.drag = 2f;
                objectRb.angularDrag = 5f;

                // ���� ���� �׸��� ����
                lineRenderer.positionCount = 2;

                Debug.Log("��ü ������� ����: " + targetObject.name + ", �Ÿ�: " + distance.ToString("F1") + "m");
            }
            else
            {
                Debug.Log("hit�� ������Ʈ�� Rigidbody�� ����: " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("����ĳ��Ʈ�� �ƹ��͵� hit���� ������");
        }
    }

    void UpdateObjectPull()
    {
        if (targetObject == null || objectSpringJoint == null)
        {
            StopObjectPull();
            return;
        }

        // ���� ���� ������Ʈ
        if (lineRenderer.positionCount == 2)
        {
            lineRenderer.SetPosition(0, GetRopeStartPosition());
            lineRenderer.SetPosition(1, targetObject.transform.position);
        }

        // �÷��̾�� ��ü ������ �Ÿ� üũ
        float distance = Vector3.Distance(targetObject.transform.position, GetRopeStartPosition());
        if (distance < 2f)
        {
            StopObjectPull();
        }

        // SpringJoint�� ���� ��ġ ������Ʈ
        if (objectSpringJoint != null)
        {
            objectSpringJoint.connectedAnchor = GetRopeStartPosition();
        }
    }

    void StopObjectPull()
    {
        if (!isPullingObject) return;

        isPullingObject = false;
        lineRenderer.positionCount = 0;

        if (objectSpringJoint != null)
        {
            Destroy(objectSpringJoint);
            objectSpringJoint = null;
        }

        targetObject = null;
        Debug.Log("��ü ������� ����!");
    }

    Vector3 GetRopeStartPosition()
    {
        if (ropeTip != null)
            return ropeTip.position;
        else if (playerTransform != null)
            return playerTransform.position;
        else
            return transform.position;
    }

    void CreateRopeTip()
    {
        if (playerTransform == null) return;

        GameObject tipObj = new GameObject("RopeTip");
        tipObj.transform.SetParent(playerTransform);
        tipObj.transform.localPosition = new Vector3(0.5f, 1.5f, 0.5f);
        ropeTip = tipObj.transform;

        Debug.Log("���� Tip�� �ڵ����� �����Ǿ����ϴ�. ���ϴ� ��ġ�� �������ּ���!");
    }

    Transform GetPlayerTransform()
    {
        // �θ� Ÿ�� �ö󰡸鼭 Rigidbody�� �ִ� ������Ʈ ã��
        Transform current = transform;
        while (current != null)
        {
            if (current.GetComponent<Rigidbody>() != null)
                return current;
            current = current.parent;
        }

        // �±׷� ã��
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            return playerObj.transform;

        return null;
    }

    void OnDrawGizmos()
    {
        Vector3 centerPos = GetRopeStartPosition();

        // Ž�� ���� ǥ�� (�����)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPos, detectionRadius);

        // �ִ� �׷��� �Ÿ� ǥ�� (�ʷϻ�)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centerPos, maxGrappleDistance);

        // ��ü ������� ���� ǥ�� (û�ϻ�)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(centerPos, objectPullRange);

        // Rope Tip ��ġ ǥ�� (�Ķ���)
        if (ropeTip != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ropeTip.position, 0.3f);
        }

        // ���� ���� Ÿ�� ǥ�� (������)
        if (bestTarget != null)
        {
            Gizmos.color = targetIndicatorColor;
            Gizmos.DrawWireSphere(bestTarget.position, 1f);
            Gizmos.DrawLine(centerPos, bestTarget.position);
        }

        // �׷��ø� ���� �� ���� ���� ǥ��
        if (isGrappling)
        {
            Gizmos.color = ropeColor;
            Gizmos.DrawLine(GetRopeStartPosition(), grapplePoint);
        }

        // ��ü ������� ���� �� ���� ���� ǥ��
        if (isPullingObject && targetObject != null)
        {
            Gizmos.color = objectPullColor;
            Gizmos.DrawLine(GetRopeStartPosition(), targetObject.transform.position);
        }
    }
}