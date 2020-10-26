﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    public GameObject Spikes;
    public GameObject Trampolin;
    private float _starTime;
    private float _intervalTime;
    private float _initTime;

    //Configuraciones de los Spikes
    private float _livingTime;
    private float _speed;
    private float _newSpeed;
    private float _timeChange;
    private bool _cutre;

    private float tramp_prob;

    // Start is called before the first frame update

    private void Awake()
    {
        tramp_prob = Configs.configuraciones.tram_prob;
    }

    void Start()
    {
        _initTime = 6f;
        _starTime = Time.time;
        _intervalTime = 0.5f;
        _livingTime = 50;
        _speed = 8;
        _newSpeed = 0.5f;
        _timeChange = 3f;
        _cutre = true;
        StartCoroutine("Generator");
        Debug.Log("Iniciado el Generador");
    }

    /*Anotaciones
     *_intervalTime = 5 =>(Spikes)=> livingTime = 60, speed = 0.5
     */
    
    void Update()
    {
        Debug.Log("Update");
        if(Time.time - _starTime >= _intervalTime || _cutre) // Crear algo cada _intervalTime
        {
            GameObject Spike;
            if(Random.value > (1-tramp_prob))
            {
                Spike = Instantiate(Trampolin, new Vector2(transform.position.x,transform.position.y+0.5f), Quaternion.identity);
                TrampolinController spikeScript = Spike.GetComponent<TrampolinController>();
                spikeScript.direction = Vector2.right;
                spikeScript.livingTime = _livingTime;
                spikeScript.speed = _speed;
                spikeScript.newSpeed = _newSpeed;
                spikeScript.timeChange = _timeChange;
            }
            else
            {
                Spike = Instantiate(Spikes, transform.position, Quaternion.identity);
                SpikesController spikeScript = Spike.GetComponent<SpikesController>();
                spikeScript.direction = Vector2.right;
                spikeScript.livingTime = _livingTime;
                spikeScript.speed = _speed;
                spikeScript.newSpeed = _newSpeed;
                spikeScript.timeChange = _timeChange;
            }
            _starTime = Time.time;
            _timeChange = Mathf.Max(0,_timeChange-0.3f);
            _cutre = false;
        }
    }

    private IEnumerator Generator()
    {

        yield return new WaitForSeconds(5.5f);
        Debug.Log("Paso el tiempo inicial" + (Time.time - _starTime) + "-" + _initTime );
        _intervalTime = 5f;
        _livingTime = 60;
        _speed = 0.5f;
        _newSpeed = _speed;
        _timeChange = _livingTime + 2;
        _cutre = true;
    }

}