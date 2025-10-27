using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위해 필요합니다.

public class SensitivityManager : MonoBehaviour
{
    public ThirdPersonPlayerController playerController;

    // 2. UI 슬라이더를 연결할 변수
    public Slider sensitivitySlider;

    void Start()
    {
        // 3. 게임 시작 시, 슬라이더의 현재 값을 
        //    플레이어의 마우스 감도(MouseSensitivity) 값으로 설정합니다.
        if (playerController != null && sensitivitySlider != null)
        {
            sensitivitySlider.value = playerController.mouseSensitivity;
        }
    }

    // 4. 슬라이더의 값이 변경될 때 호출될 공개(public) 메서드
    public void OnSensitivityChanged(float newValue)
    {
        // 5. 플레이어 컨트롤러의 MouseSensitivity 값을
        //    슬라이더의 새 값(newValue)으로 업데이트합니다.
        Debug.Log("슬라이더 값 변경됨: " + newValue);
        if (playerController != null)
        {
            playerController.mouseSensitivity = newValue;
        }
    }
}