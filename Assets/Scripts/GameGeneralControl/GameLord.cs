using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLord : MonoBehaviour
{
    public GameObject[] elements_of_game;
    // Start is called before the first frame update
    void Start()
    {
        serverManager._server_.Contextualizar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Iniciar()
    {
        for(int i = 0; i < elements_of_game.Length; ++i)
        {
            elements_of_game[i].SetActive(true);
        }
        Debug.Log("Juego Iniciado");
    }

}
