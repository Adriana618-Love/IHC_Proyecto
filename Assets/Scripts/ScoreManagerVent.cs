using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class ScoreManagerVent : MonoBehaviour
{
    public GameObject openpose;
    public GameObject ballon;
    public GameObject firepoint;
    public Text textoFback;
    public GameObject score;
    public int max_score;
    private int in_score;
    private int _divs;
    private int _hint;
    private WebCamTexture wct;


    public ShowDificult DificultyShow;
    public GameObject DifShow;
    public screencapture ObjetPose;
    // Start is called before the first frame update

    private void Awake()
    {
        max_score = Configs.configuraciones.max_puntaje;
        _divs = Mathf.RoundToInt(max_score / 3f);
        _hint = 1;
    }

    void Start()
    {
        StartCoroutine("InitGame");
        StartCoroutine("upScore");
    }

    void Desactivate(GameObject p)
    {
        staticOrderingMan script = openpose.GetComponent<staticOrderingMan>();
        //script.Deactivate();
    }

    void plot(string iS, GameObject s)
    {
        TextMeshProUGUI ss = s.GetComponent<TextMeshProUGUI>();

        ss.text = iS;
    }

    private void blind(GameObject obj)
    {
        obj.GetComponent<Renderer>().enabled = false;
    }

    private void show(GameObject obj)
    {
        obj.GetComponent<Renderer>().enabled = true;
    }

    private void Deactivate()
    {
        DifShow.SetActive(false);
    }

    IEnumerator upScore()
    {
        if (_divs * _hint <= in_score)
        {
            _hint = _hint + 1;
            if (_hint == 1)
            {
                DificultyShow.UpdateTex("Gravedad X" + _hint);
                DifShow.SetActive(true);
                Invoke("Deactivate", 1.0f);
            }
            else if (_hint == 2)
            {
                DificultyShow.UpdateTex("Gravedad X" + _hint);
                DificultyShow.text.color = Color.yellow;
                DifShow.SetActive(true);
                ballon.GetComponent<GloboSlave>().speed += _hint;
                Invoke("Deactivate", 1.0f);
            }
            else if (_hint == 3)
            {
                DificultyShow.UpdateTex("Gravedad X" + _hint);
                DificultyShow.text.color = Color.red;
                DifShow.SetActive(true);
                ballon.GetComponent<GloboSlave>().speed += _hint;
                Invoke("Deactivate", 1.0f);
            }
        }

        if (ballon.GetComponent<GloboSlave>().VIDAS <= 0)
        {
            Destroy(firepoint);
            Destroy(textoFback);
            //Desactivate(openpose);
            StartCoroutine("TheLastFail");
        }
        else if (in_score >= max_score)
        {
            Destroy(firepoint);
            Destroy(textoFback);
            //Desactivate(openpose);
            StartCoroutine("TheLastWin");
        }
        else
        {
            Debug.Log("Ploteando");
            in_score += 1;
            plot("SCORE: " + in_score.ToString(), score);
            yield return new WaitForSeconds(1);
            StartCoroutine("upScore");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CloseCamera()
    {

    }

    IEnumerator TheLastWin()
    {
        ObjetPose.cerrarSockets();
        //CloseCamera();
        //ballon.GetComponent<GloboSlave>().ES(); //Centro el globo
        ballon.GetComponent<GloboSlave>().static_speed = 0;
        score.GetComponent<Animator>().SetTrigger("win");

        /*while (ballon.transform.position.y > 0.2f || ballon.transform.position.y < -0.2f) { 
            yield return new WaitForSeconds(0.2f); 
            Debug.Log(ballon.transform.position);
        }*/
        yield return new WaitForSeconds(4f);
        score.GetComponent<Animator>().SetTrigger("finale");
        plot("Salvaste al Globo, Bien hecho", score);
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("menuScene");
    }

    IEnumerator TheLastFail()
    {
        ObjetPose.cerrarSockets();
        //CloseCamera();
        //ballon.GetComponent<GloboSlave>().ES(); //Centro el globo
        ballon.GetComponent<GloboSlave>().static_speed = 0;
        score.GetComponent<Animator>().SetTrigger("win");

        /*while (ballon.transform.position.y > 0.2f || ballon.transform.position.y < -0.2f) { 
            yield return new WaitForSeconds(0.2f); 
            Debug.Log(ballon.transform.position);
        }*/
        yield return new WaitForSeconds(4f);
        score.GetComponent<Animator>().SetTrigger("finale");
        plot("Has perdido, que triste", score);
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("menuScene");
    }

    IEnumerator InitGame()
    {

        DificultyShow.text.color = Color.black;
        DificultyShow.UpdateTex("Alcanza este puntaje: " + Configs.configuraciones.max_puntaje + "para ganar");
        DifShow.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        DifShow.SetActive(false);
    }
}