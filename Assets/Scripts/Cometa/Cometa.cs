using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cometa : MonoBehaviour
{
    private float min_Y;
    public float speed;
    public Camara camara;
    // Start is called before the first frame update
    void Start()
    {
        camara = new Camara();
        camara.GetDims();
        //Debug.Log(Left.ToString()+" " + Right.ToString()+" " + Top.ToString() +" "+ Bottom.ToString());
        min_Y = camara.Down;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= min_Y)
        {
            Destroy(this.gameObject);
        }
        transform.Translate(0,-(speed * Time.deltaTime), 0);
    }
}
