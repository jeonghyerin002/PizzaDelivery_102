using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("마우스 감도")]
    public float mouseSensitivity = 100f;

    [Header("카메라 셋팅")]
    public Transform playerBody;
    public float maxLockAngle = 80f;                   //볼 수 있는 각도 설정

    public float xRotation = 0f;                       //x축 회전을 확인하는 변수 값

    void Start()
    {
        //마우스 커머 잠그기
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //마무스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //상하 회전(카메라)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLockAngle, maxLockAngle);                  //최대 최소 움직임 각도 설정
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up, mouseX);                                            //플레이어 좌우 회전
    }
        
}
