using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometaGenerator : MonoBehaviour
{
    //server manager
    public serverManager server;

    public GameObject [] cometas;
    public float minAxisX;
    public float maxAxisX;
    public float time_generation;
    private float _init_time;
    // Start is called before the first frame update
    void Start()
    {
        server = serverManager._server_;
        _init_time = Time.time;
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        width = width / 2;
        minAxisX = -width + 1;
        maxAxisX = width - 1;
        Debug.Log(minAxisX.ToString() + " " + maxAxisX.ToString());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(server.cometas.Count > 0){
            //transform.position = new Vector2(Random.Range(minAxisX, maxAxisX), transform.position.y);
            int positionX = server.cometas[0];
            int tipoCometa = server.tipoCometas[0];
            //eliminar primeros valores
            server.cometas.RemoveAt(0);
            server.tipoCometas.RemoveAt(0);
            
            transform.position = new Vector2(positionX, transform.position.y);
            Instantiate(cometas[tipoCometa], transform.position, Quaternion.identity);
            _init_time = Time.time; //Reseteamos el tiempo
        }
    }
}
