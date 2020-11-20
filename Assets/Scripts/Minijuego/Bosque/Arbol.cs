using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arbol : MonoBehaviour
{
    Camara cam;

    private float min_Y;
    public float speed;
    void Start()
    {
        cam = new Camara();
        cam.GetDims();
        speed = 2;
    }


    void Update()
    {

        if (!cam.isBoundary(this.transform.position, 3))
        {
            Destroy(this.gameObject);
        }
        transform.Translate(0, -(speed * Time.deltaTime), 0);
    }
}
