using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BosqueController : MonoBehaviour
{
    Camara cam;
    private int _treecount;
    private float _interval;
    private float _intervalDificulty;
    private float _startTime;
    public GameObject[] arboles;
    // Start is called before the first frame update
    void Start()
    {
        _treecount = 1;
        _interval = 2;
        cam = new Camara();
        cam.GetDims();
        _startTime = Time.time;
        StartCoroutine("GenerateForest");
        _intervalDificulty = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Time.time-_startTime > _intervalDificulty && _treecount<3)
        {
            _treecount += 1;
            _startTime = Time.time;
        }
    }

    IEnumerator GenerateForest()
    {
        for(int i = 0; i < _treecount;i+=1)
        {
            Instantiate(arboles[Random.Range(0, arboles.Length)], new Vector2(Random.Range(cam.Left, cam.Right),cam.Top+2f), Quaternion.identity);
        }
        
        yield return new WaitForSeconds(_interval);
        StartCoroutine("GenerateForest");
    }
}
