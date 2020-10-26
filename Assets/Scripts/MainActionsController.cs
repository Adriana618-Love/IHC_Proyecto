using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class MainActionsController : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite[] mysprites;

    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SpriteRenderer SR = GetComponent<SpriteRenderer>();
            SR.sprite = mysprites[UnityEngine.Random.Range(0, mysprites.Length)];
        }
    }
}
