using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public GameObject Up;//1
    public GameObject Down;//3
    public GameObject Left;//2
    public GameObject Right;//0

    private int dir = 0;

    private int _status; /*-1 => Exploto, 0 => Nada, 1 => Rebote*/
    // Start is called before the first frame update

    public float energy = 1; //0 -> 1

    public Slider healthbar;

    void Start()
    {
        initPos = this.transform.position;
        direction = new Vector2(4, -4);
        speed = static_speed;
        energy = 1;
        healthbar.value = 1;
        //server manager
        server = serverManager._server_;
        SetPush();
        StartCoroutine("DownHealthBar");
    }

    public void AddEnergy()
    {
        energy += 0.3f;
        healthbar.value = energy;
    }

    private void DeactivateAll()
    {
        Up.SetActive(false);
        Right.SetActive(false);
        Left.SetActive(false);
        Down.SetActive(false);
    }

    private void SetPush()
    {
        return;
        DeactivateAll();
        if(dir == 0)
        {
            Right.SetActive(true);
        }
        else if (dir == 1)
        {
            Up.SetActive(true);
        }
        else if (dir == 2)
        {
            Left.SetActive(true);
        }
        else if (dir == 3)
        {
            Down.SetActive(true);
        }
    }

    private void addOne()
    {
        dir = (dir + 1) % 4;
    }

    private void restOne()
    {
        dir = ((dir - 1) + 4) % 4;
    }

    //Elevar
    public void Elevar_(){
        transform.Translate(0, 2, 0);
    }

    //Bajar
    public void Bajar_(){
        transform.Translate(0, -2, 0);
        Debug.Log("Bajando");
    }
    //Left
    public void Left_(){
        transform.Translate(-2, 0, 0);
    }

    //Right
    public void Right_(){
        transform.Translate(10, 0, 0);
    }

    public void GirarAntihorario_()
    {
        transform.Rotate(Vector3.back * 90);
        restOne();
        SetPush();
    }

    public void GirarHorario_()
    {
        transform.Rotate(Vector3.forward * 90);
        addOne();
        SetPush();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator DownHealthBar()
    {
        healthbar.value -= 0.02f;
        yield return new WaitForSeconds(1.0f);
        StartCoroutine("DownHealthBar");
    }
}
