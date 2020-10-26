using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometaGenerator : MonoBehaviour
{
    public GameObject [] cometas;
    public float minAxisX;
    public float maxAxisX;
    public float time_generation;
    private float _init_time;
    // Start is called before the first frame update
    void Start()
    {
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
    void Update()
    {
        if((Time.time - _init_time) > time_generation)
        {
            transform.position = new Vector2(Random.Range(minAxisX, maxAxisX), transform.position.y);
            Instantiate(cometas[Random.Range(0, cometas.Length)], transform.position, Quaternion.identity);
            _init_time = Time.time; //Reseteamos el tiempo
        }
    }
}
