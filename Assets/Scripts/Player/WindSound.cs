using UnityEngine;

public class WindSound : MonoBehaviour
{
    [Header("참조")]
    public AudioSource windSource;
    public Rigidbody rb;

    [Header("소리 설정")]
    public float minSpeed = 5.0f;  // 바람 소리가 나기 시작하는 최소 속도
    public float maxSpeed = 30.0f; // 바람 소리가 최대가 되는 속도
    public float maxVolume = 1.0f; // 최대 볼륨
    public float maxPitch = 1.5f;  // 최대 피치

    private bool isSwinging = false;

    void Update()
    {
        if (windSource == null)
            return;

        if (!isSwinging)
        {
            windSource.volume = Mathf.Lerp(windSource.volume, 0, Time.deltaTime * 5f);
            if (windSource.volume < 0.01f) windSource.Stop();
            return;
        }

        float currentSpeed = rb.velocity.magnitude;

        if (currentSpeed < minSpeed)
        {
            windSource.volume = Mathf.Lerp(windSource.volume, 0, Time.deltaTime * 5f);
            return;
        }

        if (!windSource.isPlaying) windSource.Play();

        //속도에 비례해서 0~1 사이 값(t) 구하기
        float t = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);

        //볼륨과 피치 조절
        windSource.volume = Mathf.Lerp(0, maxVolume, t);
        windSource.pitch = Mathf.Lerp(0.8f, maxPitch, t);
    }

    public void StartSwinging()
    {
        isSwinging = true;
    }

    public void StopSwinging()
    {
        isSwinging = false;
    }
}