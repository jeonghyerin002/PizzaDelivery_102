using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAction : MonoBehaviour
{
    public LayerMask grapplingObj = 1;              //그랩 되는 오브젝트 레이어
    public float maxGrappleDistanse = 100f;         //최대 그랩 거리

    public Camera playerCamera;                     //플레이어 카메라 설정

    public RaycastHit hit;

    public LineRenderer lineRenderer;
    public bool isGrappling = false;
    public Vector3 grapplePoint;

    void Start()
    {
        playerCamera = Camera.main;                                               //메인 카메라를 할당한다.

        lineRenderer = GetComponent<LineRenderer>();                              //라인 랜더러를 가져온다.
        if(lineRenderer == null)                                                  //없을 경우
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();               //컴포넌트를 생성한다.
        }

        //Line Renderer 기본 설정
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;            //처음에는 선이 없음
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
    }

    void StartGrapple()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        if(Physics.Raycast(ray, out hit, maxGrappleDistanse, grapplingObj))
        {
            grapplePoint = hit.point;
            isGrappling = true;

            //로프 라인 그리기 시작
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);                     //플레이어 위치
            lineRenderer.SetPosition(1, grapplePoint);                           //그래플 포인트
        }

        if(isGrappling)
        {
            DrawRope();
        }

        if(Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }

    void DrawRope()
    {
        if(lineRenderer.positionCount == 2)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
        }
    }

    void StopGrapple()
    {
        if (!isGrappling) return;

        isGrappling = false;
        lineRenderer.positionCount = 0;
    }
}
