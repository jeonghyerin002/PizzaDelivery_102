using UnityEngine;

public class PlayerStepSound : MonoBehaviour
{
    [Header("참조")]
    public AudioSource audioSource;
    public Rigidbody rb;
    public PlayerController playerController;

    [Header("오디오 클립")]
    public AudioClip walkClip;
    public AudioClip runClip;

    [Header("설정")]
    public float runThreshold = 15f;

    [Header("재생 속도 조절")] 
    [Range(0.1f, 3.0f)] public float walkPitch = 1.0f;
    [Range(0.1f, 3.0f)] public float runPitch = 0.8f;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (playerController == null) playerController = GetComponent<PlayerController>();

        audioSource.loop = true;
        audioSource.Stop();
    }

    void Update()
    {
        if (playerController == null)
            return;

        if (PauseMenu.isPaused || PhoneOnOff.isPhone)
        {
            if (audioSource.isPlaying) audioSource.Stop();
            return;
        }

        float currentSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        if (playerController.isGrounded && currentSpeed > 0.1f)
        {
            AudioClip targetClip;
            float targetPitch;

            if (currentSpeed > runThreshold)
            {
                targetClip = runClip;
                targetPitch = runPitch;
            }
            else
            {
                targetClip = walkClip;
                targetPitch = walkPitch;
            }

            // 클립 교체 로직
            if (audioSource.clip != targetClip || !audioSource.isPlaying)
            {
                audioSource.clip = targetClip;
                audioSource.Play();
            }

            if (audioSource.isPlaying)
            {
                audioSource.pitch = targetPitch;
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }
    }
}