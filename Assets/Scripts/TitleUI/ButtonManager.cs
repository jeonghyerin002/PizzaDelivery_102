using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button playSceneButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playSceneButton != null)
        {
            playSceneButton.onClick.AddListener(EnterPlayScene);
        }
        else
        {
            Debug.Log("버튼이 할당되지 않았습니다");
        }
    }
    void EnterPlayScene()
    {
        SceneManager.LoadScene(1);
    }
}
