using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Header("시작 설정")]
    [Tooltip("게임 시작 시 카메라의 초기 궤도 위치")]
    public Transform initialViewpoint;

    [Header("이동 및 회전 설정")]
    [Tooltip("카메라가 목표를 따라가는 속도")]
    public float moveSpeed = 5.0f;
    [Tooltip("카메라가 목표를 바라보도록 회전하는 속도")]
    public float rotateSpeed = 5.0f;

    [Header("궤도 회전 설정")]
    [Tooltip("카메라가 중심으로 돌게 될 오브젝트")]
    public Transform orbitTarget;
    [Tooltip("궤도를 도는 속도")]
    public float orbitSpeed = 10.0f;

    private Coroutine _cameraActionCoroutine;
    private Transform _dynamicTarget;

    void Awake()
    {
        _dynamicTarget = new GameObject("CameraDynamicTarget").transform;
    }

 
    void Start()
    {
        // initialViewpoint가 지정되어 있다면, 게임 시작 시 바로 궤도 회전을 시작
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