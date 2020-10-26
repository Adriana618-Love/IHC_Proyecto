using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class backOpciones : MonoBehaviour
{
    public void redirectScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void salir()
    {
        Application.Quit();
    }

    public void easy()
    {
        Configs.configuraciones.Easy();
    }
    public void medium()
    {
        Configs.configuraciones.Medium();
    }
    public void hard()
    {
        Configs.configuraciones.Hard();
    }
}
