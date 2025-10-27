using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStarter : MonoBehaviour
{
    public void EnterPlayScene()
    {
        SceneManager.LoadScene("RealMap");
    }
}
