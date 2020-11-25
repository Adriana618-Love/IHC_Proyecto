using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;

public class VentiladorSlave : MonoBehaviour
{
    //server manager
    public GameObject serverManager_;
    private serverManager server;

    public float speed;
    public Vector2 direction;
    public Sprite explosion;
    public GameObject pausa_object;
    public float minY;
    public Vector2 initPos;

    public float static_speed;

    [SerializeField]
    public screencapture screen_capture;

    private int _status; /*-1 => Exploto, 0 => Nada, 1 => Rebote*/
    // Start is called before the first frame update

    void Start()
    {
        initPos = this.transform.position;
        direction = new Vector2(4, -4);
        speed = static_speed;
        //server manager
        server = serverManager._server_;
    }

    public void GirarAntihorario_()
    {
        transform.Rotate(Vector3.back * 90);
    }

    public void GirarHorario_()
    {
        transform.Rotate(Vector3.forward * 90);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
