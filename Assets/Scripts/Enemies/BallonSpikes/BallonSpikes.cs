using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallonSpikes : MonoBehaviour
{
    Camara cam;


    void Start()
    {
        cam = new Camara();
        cam.GetDims();
    }

    
    void Update()
    {
        if (!cam.isBoundary(this.transform.position,3))
        {
            Destroy(this.gameObject);
        }
    }
}
