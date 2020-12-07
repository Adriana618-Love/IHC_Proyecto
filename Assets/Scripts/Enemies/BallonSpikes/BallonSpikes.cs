using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallonSpikes : MonoBehaviour
{
    Camara cam;
    public bool optimizate;

    private void Awake()
    {
        optimizate = true;
    }

    void Start()
    {
        cam = new Camara();
        cam.GetDims();
    }

    
    void Update()
    {
        if (!cam.isBoundary(this.transform.position,3) && optimizate)
        {
            Debug.Log("Destruido" + optimizate);
            Destroy(this.gameObject);
        }
    }
}
