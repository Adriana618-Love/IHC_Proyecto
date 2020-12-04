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
        if(serverManager._server_.rol != "N")
        {
            string myRol = "";
            if (serverManager._server_.rol == "G") myRol = "Globo";
            else if (serverManager._server_.rol == "V") myRol = "Ventilador";
            else myRol = "Esperando";
            text.text = "Rol Asignado: "+myRol;
        }
        else
        {
            yield return new WaitForSeconds(1);
            StartCoroutine("checkRol");
        }
    }
}
