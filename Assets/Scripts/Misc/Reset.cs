using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    public void RunReset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");

    }
}
