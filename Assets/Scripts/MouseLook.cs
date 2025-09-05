using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("���콺 ����")]
    public float mouseSensitivity = 100f;

    [Header("ī�޶� ����")]
    public Transform playerBody;
    public float maxLockAngle = 80f;                   //�� �� �ִ� ���� ����

    public float xRotation = 0f;                       //x�� ȸ���� Ȯ���ϴ� ���� ��

    void Start()
    {
        //���콺 Ŀ�� ��ױ�
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //������ �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //���� ȸ��(ī�޶�)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLockAngle, maxLockAngle);                  //�ִ� �ּ� ������ ���� ����
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up, mouseX);                                            //�÷��̾� �¿� ȸ��
    }
        
}
