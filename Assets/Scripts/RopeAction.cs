using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAction : MonoBehaviour
{
    public LayerMask grapplingObj = 1;              //�׷� �Ǵ� ������Ʈ ���̾�
    public float maxGrappleDistanse = 100f;         //�ִ� �׷� �Ÿ�

    public Camera playerCamera;                     //�÷��̾� ī�޶� ����

    public RaycastHit hit;

    public LineRenderer lineRenderer;
    public bool isGrappling = false;
    public Vector3 grapplePoint;

    void Start()
    {
        playerCamera = Camera.main;                                               //���� ī�޶� �Ҵ��Ѵ�.

        lineRenderer = GetComponent<LineRenderer>();                              //���� �������� �����´�.
        if(lineRenderer == null)                                                  //���� ���
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();               //������Ʈ�� �����Ѵ�.
        }

        //Line Renderer �⺻ ����
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;            //ó������ ���� ����
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

            //���� ���� �׸��� ����
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);                     //�÷��̾� ��ġ
            lineRenderer.SetPosition(1, grapplePoint);                           //�׷��� ����Ʈ
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
