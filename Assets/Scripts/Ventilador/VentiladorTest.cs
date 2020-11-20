using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentiladorTest : MonoBehaviour
{
    public GameObject Left;
    public GameObject Right;
    public GameObject Up;
    public GameObject Down;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("up"))
        {
            Up.SetActive(true);
        }

        if (Input.GetKey("down"))
        {
            Down.SetActive(true);
        }

        if (Input.GetKey("left"))
        {
            Left.SetActive(true);
        }

        if (Input.GetKey("right"))
        {
            Right.SetActive(true);
        }
        Invoke("Deactivate",1);
    }
    
    void Deactivate()
    {
        Up.SetActive(false);Down.SetActive(false); Left.SetActive(false); Right.SetActive(false);
    }
}
