using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRopeSystem : MonoBehaviour
{
    [Header("하이어라키 설정")]
    public Transform playerTransform;          // 실제 플레이어 Transform (수동 할당)
    public Transform ropeTip;                  // 로프가 나가는 위치 (손, 무기 등)

    [Header("로프 설정")]
    public LayerMask grapplingLayer = 1;       // 그래플 가능한 오브젝트 레이어
    public float maxGrappleDistance = 20f;     // 최대 그래플 거리
    public float detectionRadius = 15f;        // 탐지 반경
    public float ropeThickness = 0.1f;         // 로프 두께

    [Header("물리 설정")]
    public float spring = 4.5f;
    public float damper = 7.0f;
    public float massScale = 1.0f;
    public float ropeMinDistance = 0.25f;
    public float pullForce = 2000f;            // 플레이어 당기기 힘 (당기면 빙글빙글 돔. 아마 그래플포인트 때문인듯) 
    public float pullSpeed = 20f;              // 플레이어 당기기 최대 속도
    public float swingForce = 500f;            // 스윙 보조 힘

    [Header("물체 끌어오기 설정")]
    public float objectPullForce = 800f;       // 물체를 끌어오는 힘
    public float objectPullRange = 15f;        // 물체 끌어오기 최대 거리
    public LayerMask pullableObjects = 1;      // 끌어올 수 있는 물체 레이어

    [Header("UI 설정")]
    public Material ropeMaterial;              // 로프 재질
    public Color ropeColor = Color.white;      // 일반 로프 색상
    public Color objectPullColor = Color.cyan; // 물체 끌어오기 로프 색상
    public Color targetIndicatorColor = Color.red; // 타겟 표시 색상

    // 컴포넌트
    private LineRenderer lineRenderer;
    private SpringJoint springJoint;
    private Rigidbody playerRigidbody;
    private Camera playerCamera;

    // 로프 상태
    private bool isGrappling = false;
    private Vector3 grapplePoint;
    private GameObject currentTarget;
    private List<GameObject> nearbyTargets = new List<GameObject>();
    private Transform bestTarget;

    // 물체 끌어오기 상태
    private GameObject targetObject;
    private SpringJoint objectSpringJoint;
    private bool isPullingObject = false;

    void Start()
    {
        // 플레이어 Transform이 설정되지 않았으면 자동으로 찾기
        if (playerTransform == null)
        {
            playerTransform = GetPlayerTransform();
        }

        // 로프 Tip이 설정되지 않았으면 자동으로 생성
        if (ropeTip == null)
        {
            CreateRopeTip();
        }

        if (playerTransform != null)
        {
            playerRigidbody = playerTransform.GetComponent<Rigidbody>();
        }

        playerCamera = Camera.main;

        // LineRenderer 설정
        SetupLineRenderer();

        // 주기적으로 주변 타겟 검색
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
        // 주변 타겟 찾기 및 최적 타겟 선택
        FindBestTarget();

        // 왼쪽 마우스로 로프 발사/해제
        if (Input.GetMouseButtonDown(0))
        {
            if (!isGrappling)
                StartAutoGrapple();
            else
                StopGrapple();
        }

        // 오른쪽 마우스로 물체 끌어오기
        if (Input.GetMouseButtonDown(1))
        {
            if (!isPullingObject)
                StartObjectPull();
            else
                StopObjectPull();
        }

        // 그래플링 중일 때 추가 조작
        if (isGrappling)
        {
            UpdateRope();

            // Shift로 당기기
            if (Input.GetMouseButton(1))
                PullTowardsGrapplePoint();

            // A, D로 스윙 보조
            HandleSwingInput();
        }

        // 물체 끌어오기 중일 때
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

            // 거리 제한 확인
            if (distance > maxGrappleDistance) continue;

            // 카메라 방향과의 각도 고려
            float angle = Vector3.Angle(cameraForward, directionToTarget.normalized);
            if (angle > 90f) continue;

            // 플레이어보다 위에 있는 타겟 우선
            float heightDifference = target.transform.position.y - ropeStartPos.y;
            if (heightDifference < -2f) continue;

            // 장애물 검사
            RaycastHit hit;
            if (Physics.Raycast(ropeStartPos, directionToTarget.normalized, out hit, distance, grapplingLayer))
            {
                if (hit.collider.gameObject == target)
                {
                    // 거리와 각도를 종합한 점수 계산
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

        // 로프 라인 그리기 시작
        lineRenderer.positionCount = 2;

        // 플레이어의 Rigidbody에 SpringJoint 생성
        if (playerTransform != null && playerRigidbody != null)
        {
            springJoint = playerTransform.gameObject.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = grapplePoint;

            // 거리 계산 및 SpringJoint 설정
            float distance = Vector3.Distance(playerTransform.position, grapplePoint);
            springJoint.maxDistance = distance * 0.8f;
            springJoint.minDistance = distance * ropeMinDistance;
            springJoint.spring = spring;
            springJoint.damper = damper;
            springJoint.massScale = massScale;

            Debug.Log("로프 발사! 타겟: " + currentTarget.name + ", 거리: " + distance.ToString("F1") + "m");
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

        Debug.Log("로프 해제!");
    }

    void StartObjectPull()
    {
        // Tip 위치에서 마우스 방향으로 레이캐스트
        Vector3 tipPosition = GetRopeStartPosition();
        Ray mouseRay = playerCamera.ScreenPointToRay(Input.mousePosition);

        // Tip에서 마우스 방향으로 향하는 방향 계산
        Vector3 direction = (mouseRay.GetPoint(100f) - tipPosition).normalized;

        Debug.Log("레이캐스트 시작 위치: " + tipPosition);
        Debug.Log("레이캐스트 방향: " + direction);
        Debug.Log("레이캐스트 범위: " + objectPullRange);
        Debug.Log("타겟 레이어: " + pullableObjects.value);

        RaycastHit hit;
        // 레이어 마스크를 Physics.Raycast에 직접 적용
        if (Physics.Raycast(tipPosition, direction, out hit, objectPullRange, pullableObjects))
        {
            Debug.Log("레이캐스트 hit! 오브젝트: " + hit.collider.name + ", 레이어: " + hit.collider.gameObject.layer);

            // 플레이어 자신은 제외
            if (hit.collider.gameObject == playerTransform.gameObject)
            {
                Debug.Log("플레이어 자신을 hit했음");
                return;
            }

            // 물체에 Rigidbody가 있는지 확인
            Rigidbody objectRb = hit.collider.GetComponent<Rigidbody>();
            if (objectRb != null)
            {
                targetObject = hit.collider.gameObject;
                isPullingObject = true;

                // 물체에 SpringJoint 추가
                objectSpringJoint = targetObject.AddComponent<SpringJoint>();
                objectSpringJoint.autoConfigureConnectedAnchor = false;
                objectSpringJoint.connectedAnchor = tipPosition;

                // SpringJoint 설정 (수정된 부분)
                float distance = Vector3.Distance(targetObject.transform.position, tipPosition);
                objectSpringJoint.maxDistance = distance * 0.8f;    // 0.1f → 0.8f로 변경
                objectSpringJoint.minDistance = 1f;                 // 0f → 1f로 변경
                objectSpringJoint.spring = 200f;                    // objectPullForce(800f) → 200f로 변경
                objectSpringJoint.damper = 150f;                    // 50f → 150f로 변경
                objectSpringJoint.massScale = 0.1f;                 // 1f → 0.1f로 변경

                // 물체의 드래그 증가 (선택사항 - 더 부드럽게)
                objectRb.drag = 2f;
                objectRb.angularDrag = 5f;

                // 로프 라인 그리기 시작
                lineRenderer.positionCount = 2;

                Debug.Log("물체 끌어오기 시작: " + targetObject.name + ", 거리: " + distance.ToString("F1") + "m");
            }
            else
            {
                Debug.Log("hit한 오브젝트에 Rigidbody가 없음: " + hit.collider.name);
            }
        }
        else
        {
            Debug.Log("레이캐스트가 아무것도 hit하지 못했음");
        }
    }

    void UpdateObjectPull()
    {
        if (targetObject == null || objectSpringJoint == null)
        {
            StopObjectPull();
            return;
        }

        // 로프 라인 업데이트
        if (lineRenderer.positionCount == 2)
        {
            lineRenderer.SetPosition(0, GetRopeStartPosition());
            lineRenderer.SetPosition(1, targetObject.transform.position);
        }

        // 플레이어와 물체 사이의 거리 체크
        float distance = Vector3.Distance(targetObject.transform.position, GetRopeStartPosition());
        if (distance < 2f)
        {
            StopObjectPull();
        }

        // SpringJoint의 연결 위치 업데이트
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
        Debug.Log("물체 끌어오기 해제!");
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

        Debug.Log("로프 Tip이 자동으로 생성되었습니다. 원하는 위치로 조정해주세요!");
    }

    Transform GetPlayerTransform()
    {
        // 부모를 타고 올라가면서 Rigidbody가 있는 오브젝트 찾기
        Transform current = transform;
        while (current != null)
        {
            if (current.GetComponent<Rigidbody>() != null)
                return current;
            current = current.parent;
        }

        // 태그로 찾기
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            return playerObj.transform;

        return null;
    }

    void OnDrawGizmos()
    {
        Vector3 centerPos = GetRopeStartPosition();

        // 탐지 범위 표시 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPos, detectionRadius);

        // 최대 그래플 거리 표시 (초록색)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centerPos, maxGrappleDistance);

        // 물체 끌어오기 범위 표시 (청록색)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(centerPos, objectPullRange);

        // Rope Tip 위치 표시 (파란색)
        if (ropeTip != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ropeTip.position, 0.3f);
        }

        // 현재 최적 타겟 표시 (빨간색)
        if (bestTarget != null)
        {
            Gizmos.color = targetIndicatorColor;
            Gizmos.DrawWireSphere(bestTarget.position, 1f);
            Gizmos.DrawLine(centerPos, bestTarget.position);
        }

        // 그래플링 중일 때 로프 라인 표시
        if (isGrappling)
        {
            Gizmos.color = ropeColor;
            Gizmos.DrawLine(GetRopeStartPosition(), grapplePoint);
        }

        // 물체 끌어오기 중일 때 로프 라인 표시
        if (isPullingObject && targetObject != null)
        {
            Gizmos.color = objectPullColor;
            Gizmos.DrawLine(GetRopeStartPosition(), targetObject.transform.position);
        }
    }
}