using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        Globo.transform.position = origin;
        Paper.SetActive(true);
        Paper.transform.position = new Vector2(origin.x + (Mathf.Abs(Cam.Right - origin.x)) / 2, origin.y);
        yield return new WaitForSeconds(1);
        Globo.GetComponent<GloboController>().Right_();
        yield return new WaitForSeconds(1);
        Globo.GetComponent<GloboController>().Right_();
        ///
        yield return new WaitForSeconds(trnstion_time);
        ///////Sobrevivie el máximo de tiempo posible///////
        Globo.SetActive(false);
        Paper.SetActive(false);
        text.text = "Sobrevive el máximo de tiempo posible";
        helper.text = "SCORE: 10";
        yield return new WaitForSeconds(1);
        helper.text = "SCORE: 50";
        yield return new WaitForSeconds(1);
        helper.text = "SCORE: 100";
        ///
        yield return new WaitForSeconds(trnstion_time);

        MainCamera.transform.position = new Vector2(0, 0);
        Minigame.SetActive(true);
    }

}
