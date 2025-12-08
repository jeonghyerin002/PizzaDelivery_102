using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutScene : MonoBehaviour
{
    private PlayableDirector pd;
    public TimelineAsset[] ta;
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        pd = GetComponent<PlayableDirector>();
        playerController = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CutScene")
        {

            pd.Play();
            StartCoroutine(PausePlayer());

        }
    }

    IEnumerator PausePlayer()
    {
        playerController.pausePlayer = true;
        yield return new WaitForSeconds(11f);
        playerController.pausePlayer = false;
        SceneManager.LoadScene("RealMap");
    }
}
