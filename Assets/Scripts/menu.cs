using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public void redirectScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void salir()
    {
        Application.Quit();
    }

}
