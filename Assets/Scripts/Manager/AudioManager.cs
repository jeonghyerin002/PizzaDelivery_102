using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.UI;


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("참조")]
    public AudioMixer audioMixer;
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider masterSlider;

    public Sound[] sfxList;
    public Sound[] bgmList;

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();

    private bool isPlaylistMode = false; // 플레이리스트 모드인지 확인
    private int currentTrackIndex = 0;   // 현재 재생 중인 트랙 번호

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
            InitializeSoundLibrary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        float bgmVol = PlayerPrefs.GetFloat("BGM_Vol", 0.3f);
        float sfxVol = PlayerPrefs.GetFloat("SFX_Vol", 0.3f);
        float masterVol = PlayerPrefs.GetFloat("Master_Vol", 0.5f);

        SetBGMVolume(bgmVol);
        SetSFXVolume(sfxVol);
        SetMasterVolume(masterVol);

        if (bgmSlider != null) bgmSlider.value = bgmVol;
        if (sfxSlider != null) sfxSlider.value = sfxVol;
        if (masterSlider != null) masterSlider.value = masterVol;

        StartPlaylist();
    }

    private void Update()
    {
        if (isPlaylistMode && !bgmSource.isPlaying && bgmSource.clip != null)
        {
            // 다음 곡 재생
            PlayNextTrack();
        }
    }

    public void StartPlaylist()
    {
        if (bgmList.Length == 0) return;

        isPlaylistMode = true;
        currentTrackIndex = 0; // 0번부터 시작

        PlayPlaylistTrack(); // 첫 곡 재생
    }

    private void PlayPlaylistTrack()
    {
        if (currentTrackIndex >= bgmList.Length) currentTrackIndex = 0;

        AudioClip clip = bgmList[currentTrackIndex].clip;

        bgmSource.clip = clip;
        bgmSource.loop = false;
        bgmSource.Play();
    }
    private void PlayNextTrack()
    {
        currentTrackIndex++; // 번호 증가

        if (currentTrackIndex >= bgmList.Length)
        {
            currentTrackIndex = 0;
        }

        PlayPlaylistTrack();
    }

    private void InitializeSoundLibrary()
    {
        foreach (Sound s in sfxList)
        {
            if (s.clip != null && !sfxDictionary.ContainsKey(s.name))
                sfxDictionary.Add(s.name, s.clip);
        }
        foreach (Sound s in bgmList)
        {
            if (s.clip != null && !bgmDictionary.ContainsKey(s.name))
                bgmDictionary.Add(s.name, s.clip);
        }
    }

    //배경음악 재생
    public void PlayBGM(string bgmName)
    {
        if (bgmDictionary.ContainsKey(bgmName))
        {
            AudioClip clipToPlay = bgmDictionary[bgmName];

            if (bgmSource.clip == clipToPlay) return;       //같은 bgm일 경우 리턴

            bgmSource.clip = clipToPlay;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM 파일 없음: {bgmName}");
        }
    }

    // 효과음 재생
    public void PlaySFX(string sfxName)
    {
        if (sfxDictionary.ContainsKey(sfxName))
        {
            sfxSource.PlayOneShot(sfxDictionary[sfxName]);
        }
        else
        {
            Debug.LogWarning($"SFX 파일 없음: {sfxName}");
        }
    }


    //볼륨 조절
    public void SetBGMVolume(float sliderValue)
    {
        float db = (sliderValue <= 0.001f) ? -80f : Mathf.Log10(sliderValue) * 20;
        audioMixer.SetFloat("BGM", db);

        PlayerPrefs.SetFloat("BGM_Vol", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float db = (sliderValue <= 0.001f) ? -80f : Mathf.Log10(sliderValue) * 20;
        audioMixer.SetFloat("SFX", db);

        PlayerPrefs.SetFloat("SFX_Vol", sliderValue);
    }

    public void SetMasterVolume(float sliderValue)
    {
        float db = (sliderValue <= 0.001f) ? -80f : Mathf.Log10(sliderValue) * 20;
        audioMixer.SetFloat("Master", db);

        PlayerPrefs.SetFloat("Master_Vol", sliderValue);
    }
}