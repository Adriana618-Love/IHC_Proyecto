﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolinController : MonoBehaviour
{
    public float speed;
    public Vector2 direction;

    public float livingTime = 60;
    public float timeChange;
    public float newSpeed;
    private float _startTime;
    private Animator _animator;

    private void Awake()
    {
        _animator = this.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, livingTime);
        _startTime = Time.time;
    }

    void Change()
    {
        if (Time.time - _startTime > timeChange)
        {
            Debug.Log("Cambio");
            speed = newSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _animator.SetTrigger("rebota");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = direction.normalized * speed * Time.deltaTime;

        //transform.position = new Vector2(transform.position.x + movement.x, transform.position.y + movement.y);
        transform.Translate(movement);
        Change();
    }
}