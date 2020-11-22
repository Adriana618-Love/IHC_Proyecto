using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject Balloon;
    private GameObject Game;
    // Start is called before the first frame update
    void Start()
    {
        Game = GameObject.Find("Game");
        Balloon = Game.transform.Find("Balloon").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
