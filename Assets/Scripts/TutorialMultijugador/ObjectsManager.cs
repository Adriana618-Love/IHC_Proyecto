﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ObjectsManager : MonoBehaviour
{
    #region Variables
    private float trnstion_time = 2.0f;
    public TextMeshProUGUI text;
    public TextMeshProUGUI helper;

    public GameObject Ventilador;
    public GameObject Globo;

    public GameObject Globo_enpinchado;
    public GameObject Cometas;
    public GameObject Paper;

    public GameObject MainCamera;
    public GameObject Minigame;
    public Camara Cam;
    public GameObject Desconection;
    public GameObject Conection;
    public GameObject Pila;
    #endregion

    public Vector2 origin;


    // Start is called before the first frame update
    void Start()
    {
        Cam = new Camara();
        origin = MainCamera.transform.position;
        Cam.GetDims();
        StartCoroutine("Tutorial");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Flip(GameObject O)
    {
        float localScaleX = O.transform.localScale.x;
        localScaleX = localScaleX * -1f;
        O.transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        float fa = O.GetComponent<AreaEffector2D>().forceAngle;
        O.GetComponent<AreaEffector2D>().forceAngle = (fa + 180) % 360;
    }

    private void Stop(GameObject O)
    {
        O.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        O.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
    }

    IEnumerator Tutorial()
    {
        Debug.Log("Empieza tutorial");
        ///////Hola///////
        text.text = "Hola";
        text.transform.position = origin;
        ///
        yield return new WaitForSeconds(trnstion_time);
        ///////Bienvenido a Multijugador///////
        text.text = "Bienvenido a Multijugador";
        text.transform.position = origin;
        text.transform.Rotate(0.0f,0.0f,45.0f,Space.Self);
        ///
        yield return new WaitForSeconds(trnstion_time);
        ///////Tienes 2 personajes///////
        text.text = "Tienes a 2 personajes";
        text.transform.Rotate(0.0f, 0.0f, -45.0f, Space.Self);
        text.transform.position = new Vector2(origin.x,origin.y + (Mathf.Abs( Cam.Top - origin.y))/2);
        Globo.SetActive(true);
        Ventilador.SetActive(true);
        Globo.transform.position = new Vector2(origin.x + (Mathf.Abs(Cam.Right - origin.x))/2, origin.y);
        Ventilador.transform.position = new Vector2(origin.x - (Mathf.Abs(-Cam.Left + origin.x))/2, origin.y);
        ///
        yield return new WaitForSeconds(trnstion_time);
        ///////Ventilador///////
        text.text = "Ventilador";
        Globo.SetActive(false);
        Ventilador.transform.position = origin;
        ///
        yield return new WaitForSeconds(trnstion_time);
        ///////Despeja el campo///////
        text.text = "Despeja el campo";
        Ventilador.transform.position = new Vector2(origin.x - (Mathf.Abs(-Cam.Left + origin.x)) / 2, origin.y + (Mathf.Abs(Cam.Top - origin.y)) / 2);
        Globo_enpinchado.SetActive(true);
        Globo_enpinchado.GetComponent<BallonSpikes>().optimizate = false;
        Globo_enpinchado.transform.position = new Vector2(origin.x, origin.y + (Mathf.Abs(Cam.Top - origin.y)) / 2);
        yield return new WaitForSeconds(1);
        Stop(Globo_enpinchado);
        Ventilador.transform.position = new Vector2(origin.x + (Mathf.Abs(Cam.Right - origin.x)) / 2, origin.y - (Mathf.Abs(-Cam.Down + origin.y)) / 2);
        Flip(Ventilador);
        Globo_enpinchado.transform.position = new Vector2(origin.x, origin.y - (Mathf.Abs(Cam.Top - origin.y)) / 2);
        ///
        yield return new WaitForSeconds(trnstion_time);
        ///////Cuidado con el Globo///////
        text.text = "Cuidado con el Globo";
        Stop(Globo_enpinchado);
        Flip(Ventilador);
        Globo.SetActive(true);
        Globo.transform.position = new Vector2(origin.x + (Mathf.Abs(Cam.Right - origin.x)) / 2, origin.y + (Mathf.Abs(Cam.Top - origin.y)) / 2);
        Globo_enpinchado.transform.position = new Vector2(origin.x, origin.y + (Mathf.Abs(Cam.Top - origin.y)) / 2);
        Ventilador.transform.position = new Vector2(origin.x - (Mathf.Abs(-Cam.Left + origin.x)) / 2, origin.y + (Mathf.Abs(Cam.Top - origin.y)) / 2);
        ///
        yield return new WaitForSeconds(trnstion_time);
        ///////Recolecta los paper para vidas extra///////
        text.text = "Recolecta las pilas para mantener tu energía";
        Ventilador.transform.position = new Vector2(origin.x - (Mathf.Abs(Cam.Right - origin.x)) / 2, origin.y);
        Pila.SetActive(true);
        Pila.transform.position = new Vector2(origin.x + (Mathf.Abs(Cam.Right - origin.x)) / 2, origin.y);
        yield return new WaitForSeconds(1);
        Ventilador.GetComponent<VentiladorController>().Right_();
        Pila.SetActive(false);
        yield return new WaitForSeconds(1);
        //Ventilador.GetComponent<GloboController>().Right_();
        Ventilador.SetActive(false);
        ///
        yield return new WaitForSeconds(trnstion_time);
        ///////Globo///////
        text.text = "Globo";
        Globo_enpinchado.SetActive(false);
        Ventilador.SetActive(false);
        Stop(Globo);
        Globo.transform.position = origin;
        ///
        yield return new WaitForSeconds(trnstion_time);
        ///////Esquiva las cometas///////
        text.text = "Esquiva las cometas";
        Cometas.SetActive(true);
        Cometas.transform.position = new Vector2(origin.x, Cam.Top);
        Globo.transform.position = new Vector2(origin.x, origin.y - (Mathf.Abs(-Cam.Down + origin.y)) / 2);
        while (Vector3.Distance(Cometas.transform.position, Globo.transform.position) > 4)
        {
            Debug.Log(Vector3.Distance(Cometas.transform.position, Globo.transform.position));
            yield return new WaitForSeconds(0.1f);
        }
        Globo.GetComponent<GloboController>().Left_();
        ///
        yield return new WaitForSeconds(trnstion_time);
        ///////Recolecta los paper para vidas extra///////
        Cometas.SetActive(false);
        text.text = "Recolecta los paper para vidas extra";
        Globo.transform.position = new Vector2(origin.x, origin.y-1);
        Paper.SetActive(true);
        Paper.transform.position = new Vector2(origin.x + (Mathf.Abs(Cam.Right - origin.x)) / 2, origin.y);
        yield return new WaitForSeconds(1);
        Globo.GetComponent<GloboController>().Right_();
        yield return new WaitForSeconds(1);
        Globo.GetComponent<GloboController>().Right_();
        ///
        yield return new WaitForSeconds(trnstion_time);
        ///////Sobrevivie el máximo de tiempo posible///////
        text.text = "Sobrevivie el máximo de tiempo posible";
        Globo.SetActive(false);
        Paper.SetActive(false);
        ////////////////////////////////////////////////////
        yield return new WaitForSeconds(trnstion_time);
        text.text = "";
        /////////////Verifica tu conexión///////////////
        text.text = "Verifica tu conexión";
        Conection.SetActive(true);
        Conection.transform.position = origin;
        //////////////////////////////////////
        yield return new WaitForSeconds(trnstion_time);
        text.text = "";
        Conection.SetActive(false);
        /////////////Conectado////////////////
        text.text = "Conectado";
        Desconection.SetActive(true);
        Desconection.transform.position = new Vector2(origin.x, origin.y-1);
        yield return new WaitForSeconds(trnstion_time);
        /////////////Desconectado//////////////
        text.text = "Desconectado";
        Desconection.transform.position = new Vector2(origin.x, origin.y - 1);
        Desconection.GetComponent<Animator>().SetTrigger("click");
        ///////////////////////////////////////
        yield return new WaitForSeconds(trnstion_time);
        Desconection.SetActive(false);
        /////////////Globo sigue jugando///////
        text.text = "Si el ventilador se desconecta, el Globo sigue jugando";
        Globo.SetActive(true);
        Globo.transform.position = new Vector2(origin.x, origin.y - (Mathf.Abs(-Cam.Down + origin.y)) / 2);
        ///////////////////////////////////////

        yield return new WaitForSeconds(trnstion_time);
        Globo.SetActive(false);
        ////////////Ventilador se desconecta//////////////
        text.text = "Si el glob se desconecta, el Ventilador no sigue jugando";
        Ventilador.SetActive(true);
        Ventilador.transform.position = new Vector2(origin.x, origin.y - (Mathf.Abs(-Cam.Down + origin.y)) / 2);
        yield return new WaitForSeconds(trnstion_time);
        Ventilador.SetActive(false);
        /////////////////////////////////////////////////
        yield return new WaitForSeconds(trnstion_time);
        text.text = "";
        ///////////Movimientos de Ventilador/////////////
        text.text = "Grita Go para que el Ventilador avance";
        Ventilador.SetActive(true);
        Ventilador.transform.position = new Vector2(origin.x, origin.y - (Mathf.Abs(-Cam.Down + origin.y)) / 2);
        while (Ventilador.transform.position == new Vector3(origin.x, origin.y - (Mathf.Abs(-Cam.Down + origin.y)) / 2, Ventilador.transform.position.z))
        {
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(trnstion_time);
        /////////////////////////////////////////////////
        yield return new WaitForSeconds(trnstion_time);
        SceneManager.LoadScene("menuScene");
    }

}
