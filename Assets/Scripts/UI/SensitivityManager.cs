using UnityEngine;
using UnityEngine.UI; // UI ��Ҹ� ����ϱ� ���� �ʿ��մϴ�.

public class SensitivityManager : MonoBehaviour
{
    public ThirdPersonPlayerController playerController;

    // 2. UI �����̴��� ������ ����
    public Slider sensitivitySlider;

    void Start()
    {
        // 3. ���� ���� ��, �����̴��� ���� ���� 
        //    �÷��̾��� ���콺 ����(MouseSensitivity) ������ �����մϴ�.
        if (playerController != null && sensitivitySlider != null)
        {
            sensitivitySlider.value = playerController.mouseSensitivity;
        }
    }

    // 4. �����̴��� ���� ����� �� ȣ��� ����(public) �޼���
    public void OnSensitivityChanged(float newValue)
    {
        // 5. �÷��̾� ��Ʈ�ѷ��� MouseSensitivity ����
        //    �����̴��� �� ��(newValue)���� ������Ʈ�մϴ�.
        Debug.Log("�����̴� �� �����: " + newValue);
        if (playerController != null)
        {
            playerController.mouseSensitivity = newValue;
        }
    }
}