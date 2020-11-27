using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public GameObject openpose;
    public GameObject ballon;
    public GameObject firepoint;
    public Text textoFback;
    public GameObject score;
    public int max_score;
    private int in_score;
    private WebCamTexture wct;

    public GameObject configs;

    // Start is called before the first frame update

    private void Awake()
    {
        max_score = Configs.configuraciones.max_puntaje;
    }

    void Start()
    {
        wct = new WebCamTexture();
        StartCoroutine("upScore");
        in_score = 0;
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

    IEnumerator upScore()
    {
        if (in_score >= max_score)
        {
            Destroy(firepoint);
            Destroy(textoFback);
            Desactivate(openpose);
            StartCoroutine("TheLast");
        }
        else
        {
            //Debug.Log("Ploteando");
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
        if (wct.isPlaying)
            wct.Stop();
    }

    IEnumerator TheLast()
    {
        CloseCamera();
        if (serverManager._server_.rol == "G")
            ballon.GetComponent<GloboController>().ES(); //Centro el globo
        else
            ballon.GetComponent<GloboSlave>().ES();
        score.GetComponent<Animator>().SetTrigger("win");
        
        while (ballon.transform.position.y > 0.2f || ballon.transform.position.y < -0.2f) { 
            yield return new WaitForSeconds(0.2f); 
            Debug.Log(ballon.transform.position);
        }
        yield return new WaitForSeconds(4f);
        score.GetComponent<Animator>().SetTrigger("finale");
        plot("Salvaste al Globo, Bien hecho", score);
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("menuScene");
    }
}
