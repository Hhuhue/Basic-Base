using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HomeController
{
    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }
}
