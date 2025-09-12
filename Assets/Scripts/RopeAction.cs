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

    public Transform player;
    public float spring = 4.5f;
    public float damper = 7.0f;
    public float massScale = 1.0f;
    public float ropeMinDistanse = 0.25f;

    public SpringJoint springJoint;
    public Rigidbody playerRigidbody;

    //���� ���
    public float pullForce = 1000.0f;
    public float pullSpeed = 20f;

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

        if (Input.GetMouseButton(1) && isGrappling)             //��Ŭ������ ����
        {
            PulltowrdsGrapplePoint();
        }

        if (isGrappling)
        {
            DrawRope();
        }
    }

    void StartGrapple()
    {
        if (isGrappling) return;                           //�̹� �׷��ø� ���̸� ����
        
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        if(Physics.Raycast(ray, out hit, maxGrappleDistanse, grapplingObj))
        {
            grapplePoint = hit.point;
            isGrappling = true;

            //���� ���� �׸��� ����
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);                     //�÷��̾� ��ġ
            lineRenderer.SetPosition(1, grapplePoint);                           //�׷��� ����Ʈ

            //������ ����Ʈ ����
            springJoint = player.gameObject.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = grapplePoint;

            //�Ÿ����
            float distance = Vector3.Distance(player.position, grapplePoint);

            //������ ����Ʈ ����
            springJoint.maxDistance = distance * 0.8f;
            springJoint.minDistance = distance * ropeMinDistanse;
            springJoint.spring = spring;
            springJoint.damper = damper;
            springJoint.massScale = massScale;
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

        if (springJoint != null)                                   //������ ����Ʈ ����
        {
            Destroy(springJoint);
            springJoint = null;
        }
    }

    void PulltowrdsGrapplePoint()
    {
        if(!isGrappling || playerRigidbody == null) return;

        //�÷��̾�� �׷��� ����Ʈ���� ���� ���
        Vector3 directionToGrapple = (grapplePoint - player.position).normalized;

        //���� �ӵ��� ��ǥ ���⳻�� �������� �̹� �� �������� �����̰� �ִ��� Ȯ��
        float currentVelocityInDirection = Vector3.Dot(playerRigidbody.velocity, directionToGrapple);

        //�ִ� �ӵ��� ������ ���� ���
        if(currentVelocityInDirection < pullSpeed)
        {
            Vector3 pullForceVector = directionToGrapple * pullForce * Time.deltaTime;
            playerRigidbody.AddForce(pullForceVector, ForceMode.Force);
        }
    }
}
