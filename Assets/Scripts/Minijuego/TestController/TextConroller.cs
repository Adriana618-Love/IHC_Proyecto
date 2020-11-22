using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextConroller : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("checkRol");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator checkRol()
    {
        if(serverManager._server_.rol[0] != 'N')
        {
            string myRol = "";
            if (serverManager._server_.rol[1] == 'G') myRol = "Globo";
            else myRol = "Ventilador";
            text.text = "Rol Asignado: "+myRol;
        }
        else
        {
            yield return new WaitForSeconds(1);
            StartCoroutine("checkRol");
        }
    }
}
