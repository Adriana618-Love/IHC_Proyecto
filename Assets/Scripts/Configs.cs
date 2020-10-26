using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configs : MonoBehaviour
{
    public static Configs configuraciones;
    public int max_puntaje;
    public float tram_prob;
    public float speed_globo;

    private void Awake()
    {
        if (configuraciones == null)
        {
            configuraciones = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(configuraciones!=this)
        {
            Destroy(this.gameObject);
        }
    }

    public void Easy()
    {
        max_puntaje = 50;
        tram_prob = 0.8f;
        speed_globo = 0.5f;
        Debug.Log("easy");
    }

    public void Medium()
    {
        max_puntaje = 100;
        tram_prob = 0.5f;
        speed_globo = 0.7f;
        Debug.Log("medio");
    }

    public void Hard()
    {
        max_puntaje = 100;
        tram_prob = 0.2f;
        speed_globo = 1f;
        Debug.Log("dificil");
    }

}
