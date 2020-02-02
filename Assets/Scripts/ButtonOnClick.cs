using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonOnClick : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
