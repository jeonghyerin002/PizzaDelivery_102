using Cinemachine;
using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("참조")]
    public LineRenderer lineRenderer;
    public LayerMask swingableLayer; // 스윙 가능한 오브젝트의 레이어
    public PlayerController playerController;           //PlayerController 참조
    public Transform hand;
    public StaminaSystem staminaSystem;
    public CinemachineVirtualCamera cam;

    [HideInInspector]
    public bool isSwinging = false;

    [Header("스윙 설정")]
    public float maxSwingDistance = 50f; // 최대 스윙 거리
    public float swingSpring = 100f;     // 스프링 탄성 (높을수록 뻣뻣함)
    public float swingDamper = 10f;      // 스프링 저항 (진동을 줄임)
    public float swingMassScale = 4.5f;  // 조인트에 연결된 질량 스케일
    public float swingRewind = 10f;     //1초에 줄을 감을 값
    public float swingPull = 20f;       //스윙 추진력
    public float defaultFOV = 70f;
    public float swingFOV = 90f;

    [Header("스테미나 설정")]
    public float startSwingCost = 5f;  //스윙 시작 스테미나 소모량
    public float swingPullStaminaCost = 5f;    //줄 감기 스테미나 소모량

    [Header("공중 컨트롤")]
    public float airControlForce = 10f; // 스윙 중 공중 컨트롤 힘
    public float swingReleaseBoost = 5f; // 스윙을 끊을 때 추가할 힘
    public float backFlipSpeed = 20f;   //이 속도가 넘어가면 백플립

    private Rigidbody playerRigidbody;
    private SpringJoint springJoint;
    private Vector3 swingPoint;
    private float targetFOV;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        lineRenderer.enabled = false;
        playerController = GetComponent<PlayerController>();
        staminaSystem = GetComponent<StaminaSystem>();

        targetFOV = defaultFOV;
    }

    void Update()
    {
        if (PauseMenu.isPaused) return;      //게임 일시정지 상태면 return
        if (PhoneOnOff.isPhone) return;     //휴대폰이 켜지면 return
        // 마우스 왼쪽 버튼 클릭 시 스윙 시작
        if (Input.GetMouseButtonDown(0))
        {
            StartSwing();
            targetFOV = swingFOV;
        }

        // 마우스 왼쪽 버튼 뗄 시 스윙 중지
        if (Input.GetMouseButtonUp(0))
        {
            StopSwing();
            targetFOV = defaultFOV;
        }

        float currentFOV = cam.m_Lens.FieldOfView;
        cam.m_Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * 5f);

    }

    void FixedUpdate()
    {
        // 스윙 중에만 공중 컨트롤 적용
        if (springJoint != null && !playerController.isGrounded)
        {
            // 키보드 입력(W, A, S, D)으로 공중에서 힘 적용
            float horizontal = Input.GetAxis("Horizontal"); // A, D
            float vertical = Input.GetAxis("Vertical");     // W, S

            Vector3 cameraForward = playerController.mainCamera.transform.forward;
            Vector3 cameraRight = playerController.mainCamera.transform.right;

            // y축 영향 제거하여 수평 방향으로만 힘 적용
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 forceDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;
            playerRigidbody.AddForce(forceDirection * airControlForce, ForceMode.Acceleration);
        }

        if (Input.GetMouseButton(1)) //줄 감기
        {
            if (springJoint != null)
            {
                if (staminaSystem.DrainStamina(swingPullStaminaCost * Time.fixedDeltaTime))
                {
                    springJoint.maxDistance -= Time.fixedDeltaTime * swingRewind;
                    springJoint.maxDistance = Mathf.Max(springJoint.maxDistance, springJoint.minDistance);

                    Vector3 dirToPoint = (swingPoint - hand.position).normalized;
                    playerRigidbody.AddForce(dirToPoint * swingPull, ForceMode.Acceleration);
                }
            }


        }
    }
    void LateUpdate()
    {
        DrawRope();
    }

    void StartSwing()
    {
        if (staminaSystem.currentStamina <= 0) return;      //스테미나 없으면 스윙 불가
        // 카메라 정면으로 Raycast 발사
        RaycastHit hit;
        if (Physics.Raycast(playerController.mainCamera.transform.position, playerController.mainCamera.transform.forward, out hit, maxSwingDistance, swingableLayer))
        {
            if (staminaSystem.DrainStamina(startSwingCost))
            {
                AudioManager.instance.PlaySFX("Wind");
                swingPoint = hit.point; // 맞은 지점을 스윙 포인트로 저장
                isSwinging = true;
                playerController.animator.SetBool("isSwinging", true);

                // SpringJoint 컴포넌트 추가 및 설정
                springJoint = gameObject.AddComponent<SpringJoint>();
                springJoint.autoConfigureConnectedAnchor = false; // 연결 지점을 수동 설정
                springJoint.connectedAnchor = swingPoint;         // 연결 지점은 Raycast가 맞은 곳

                // 플레이어와 스윙 포인트 사이의 거리를 계산
                float distanceFromPoint = Vector3.Distance(hand.position, swingPoint);
                springJoint.anchor = transform.InverseTransformPoint(hand.position); //조인트 시작점을 손으로

                // maxDistance를 현재 거리로 설정하여 줄이 팽팽하게 시작하도록 함
                springJoint.maxDistance = distanceFromPoint * 0.9f; // 약간의 여유를 주어 당겨지는 느낌
                springJoint.minDistance = distanceFromPoint * 0f; // 최소 거리

                // 스프링 값 설정
                springJoint.spring = swingSpring;
                springJoint.damper = swingDamper;
                springJoint.massScale = swingMassScale;

                // 라인 렌더러 활성화 및 위치 설정
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, hand.position); // 시작점: 플레이어
                lineRenderer.SetPosition(1, swingPoint);         // 끝점: 스윙 포인트
            }
            else
            {
                Debug.Log("스테미나 부족");
            }

        }
    }

    void StopSwing()
    {
        // 라인 렌더러 비활성화
        lineRenderer.enabled = false;

        // SpringJoint가 존재하면 제거
        if (springJoint != null)
        {
            Destroy(springJoint);
            springJoint = null;
            isSwinging = false;

            float currentSpeed = playerRigidbody.velocity.magnitude;        //속도 확인
            playerController.animator.SetBool("isSwinging", false);

            if (currentSpeed > backFlipSpeed)
            {
                // 백플립 애니메이션 트리거 발동
                playerController.animator.SetTrigger("backFlip");
            }

            //스윙을 끊을때 부스트 적용
            playerRigidbody.AddForce(playerController.mainCamera.transform.forward * swingReleaseBoost, ForceMode.Impulse);
        }
    }

    void DrawRope()
    {
        // 스윙 중이 아니거나 라인 렌더러가 비활성화면 아무것도 안 함
        if (springJoint == null || !lineRenderer.enabled) return;

        // 라인의 시작점을 플레이어의 현재 위치로 계속 업데이트
        lineRenderer.SetPosition(0, hand.position);
    }
}