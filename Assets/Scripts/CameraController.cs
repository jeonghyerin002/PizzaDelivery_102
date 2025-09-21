using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Header("���� ����")]
    [Tooltip("���� ���� �� ī�޶��� �ʱ� �˵� ��ġ")]
    public Transform initialViewpoint;

    [Header("�̵� �� ȸ�� ����")]
    [Tooltip("ī�޶� ��ǥ�� ���󰡴� �ӵ�")]
    public float moveSpeed = 5.0f;
    [Tooltip("ī�޶� ��ǥ�� �ٶ󺸵��� ȸ���ϴ� �ӵ�")]
    public float rotateSpeed = 5.0f;

    [Header("�˵� ȸ�� ����")]
    [Tooltip("ī�޶� �߽����� ���� �� ������Ʈ")]
    public Transform orbitTarget;
    [Tooltip("�˵��� ���� �ӵ�")]
    public float orbitSpeed = 10.0f;

    private Coroutine _cameraActionCoroutine;
    private Transform _dynamicTarget;

    void Awake()
    {
        _dynamicTarget = new GameObject("CameraDynamicTarget").transform;
    }

 
    void Start()
    {
        // initialViewpoint�� �����Ǿ� �ִٸ�, ���� ���� �� �ٷ� �˵� ȸ���� ����
        if (initialViewpoint != null)
        {
            StartMoveAndOrbit(initialViewpoint);
        }
    }

    public void StartMoveAndOrbit(Transform targetViewpoint)
    {
        _dynamicTarget.position = targetViewpoint.position;
        if (_cameraActionCoroutine != null)
        {
            StopCoroutine(_cameraActionCoroutine);
        }
        _cameraActionCoroutine = StartCoroutine(TrackAndOrbit());
    }

    private IEnumerator TrackAndOrbit()
    {
        while (true)
        {
            if (orbitTarget != null)
            {
                _dynamicTarget.RotateAround(orbitTarget.position, Vector3.up, orbitSpeed * Time.deltaTime);
            }
            transform.position = Vector3.Lerp(transform.position, _dynamicTarget.position, moveSpeed * Time.deltaTime);
            if (orbitTarget != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(orbitTarget.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }
}