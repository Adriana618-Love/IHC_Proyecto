using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;
//using UnityEditor.U2D.Sprites;
using UnityEditor;

public class GloboTutorial : MonoBehaviour
{
    public float speed;
    public Vector2 direction;
    public Sprite explosion;
    public Sprite normal;
    public string scene;
    public GameObject microphone;
    public GameObject welldonemsg;
    public GameObject jumboTron;

    private KeywordRecognizer _keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    private bool _izqTrain = false;
    private bool _derTrain = false;

    public Text texto;
    public Text t_support;
    public Sprite indi1;//Alejate
    public Sprite indi2;//Hablaras

    public GameObject pinchos;
    public GameObject trampolin;

    public AudioSource[] sounds;
    public AudioSource trampolinAudio;
    public AudioSource estallandoAudio;
    private Animator animator;

    private int _status; /*-1 => Exploto, 0 => Nada, 1 => Rebote*/

    private void Awake()
    {
        sounds = GetComponents<AudioSource>();
        estallandoAudio = sounds[0];
        trampolinAudio = sounds[1];
        animator = this.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //direction = new Vector2(4, -4);
        //speed = 2;
        _status = 0;
        actions.Add("izquierda", Left);
        actions.Add("derecha", Right);

        _keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        _keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        _keywordRecognizer.Start();
        StartCoroutine("Tutorial");
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void Left()
    {
        transform.Translate(-2.5f, 0, 0);
        _izqTrain = true;
    }

    private void Right()
    {
        transform.Translate(2.5f, 0, 0);
        _derTrain = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Spikes(Clone)")
        {
            Debug.Log("Explosion");
            direction.y = direction.x = 0;
            estallandoAudio.Play();
            animator.SetTrigger("pum");
            _status = -1;
        }
        else if(collision.gameObject.name == "Trampolin(Clone)")
        {
            Debug.Log("Rebota");
            this.transform.position = new Vector2(transform.position.x, transform.position.y + 4);
            trampolinAudio.Play();
            _status = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        direction.x *= -1;
        Vector2 movement = direction.normalized * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    private void blind(GameObject obj)
    {
        obj.GetComponent<Renderer>().enabled = false;
    }

    private void show(GameObject obj)
    {
        obj.GetComponent<Renderer>().enabled = true;
    }

    private void assingSprite(GameObject obj, Sprite spt)
    {
        obj.GetComponent<SpriteRenderer>().sprite = spt;
    }

    private IEnumerator Tutorial()
    {
        blind(this.gameObject);
        texto.text = "";
        t_support.text = "";
        //Indicaciones Básicas
        ///Aléjate de la pantalla
        texto.fontSize = 30+40;
        texto.transform.position = new Vector2(100f,300+400f);
        texto.text = "Acomoda tu laptop y alejate lo suficiente";
        assingSprite(jumboTron, indi1);
        yield return new WaitForSeconds(5f);
        ///Tranquilo, no necesitarás las manos
        texto.fontSize = 30+40;
        texto.transform.position = new Vector2(90f, 300+400f);
        texto.text = "Tranquilo, no necesitaras las manos, sino tu voz";
        assingSprite(jumboTron, indi2);
        yield return new WaitForSeconds(5f);
        //Transición a comandos por voz
        Destroy(jumboTron);//Adiós JumboTron
        texto.fontSize = 40+40;
        texto.transform.position = new Vector2(200+350, 150+350f);
        texto.text = "Probemos tu hermosa voz";
        yield return new WaitForSeconds(3f);
        //Entrenar izquierda
        show(this.gameObject);
        GameObject micro = Instantiate(microphone, new Vector2(6.5f, -1.89f), Quaternion.identity);
        texto.text = "DI IZQUIERDA";
        texto.fontSize = 70+40;
        texto.transform.position = new Vector2(-192f + 405f+350f, -80f + 182.5f+200f);
        while (_izqTrain == false)
        {
            yield return null;
        }
        blind(this.gameObject);
        blind(micro);
        texto.text = "";
        GameObject wdm = Instantiate(welldonemsg, new Vector2(1f, 1f), Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Destroy(wdm);
        //Entrenar Derecha
        show(this.gameObject);
        show(micro);
        texto.text = "DI DERECHA";
        while (_derTrain == false)
        {
            yield return null;
        }
        _keywordRecognizer.Stop();
        Destroy(micro);
        blind(this.gameObject);
        texto.text = "";
        wdm = Instantiate(welldonemsg, new Vector2(1f, 1f), Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Destroy(wdm);
        //Advertir sobre los eventos en el juego
        show(this.gameObject);
        texto.transform.position = new Vector2(100+350, 300+400);
        texto.fontSize = 40+20;
        texto.text = "OBSERVA LAS MECÁNICAS DEL JUEGO";
        ///Globo que cae sobre los pinchos
        ////////////////////////////////////////////////////
        ///Creamos los pinchos
        t_support.text = "CUIDADO CON LAS COMETAS!";
        t_support.fontSize = 30 + 40;
        GameObject spikes = Instantiate(pinchos, new Vector2(-1.0859f, -3.923067f), Quaternion.identity);
        spikes.GetComponent<SpikesController>().speed = 0f;
        ////////////////////////////////////////////////////
        this.transform.position = new Vector2(-0.85f, 0.75f);
        direction = new Vector2(4, -4);
        speed = 2;
        while (direction.y != 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        Destroy(spikes);
        ///Globo que cae sobre el trampolín
        ///////////////////////////////////////////////////////
        ///Creamos el trampolín
        t_support.text = "USA LOS TRAMPOLINES";
        t_support.fontSize = 30+40;
        GameObject elasticBed = Instantiate(trampolin, new Vector2(1.72f, -4.47f), Quaternion.identity);
        ////////////////////////////////////////////////////
        SpriteRenderer ms = this.GetComponent<SpriteRenderer>();
        ms.sprite = normal;
        this.transform.position = new Vector2(1.80f, 0.75f);
        direction = new Vector2(4, -4);
        speed = 2;
        yield return new WaitForSeconds(6.8f);
        Destroy(elasticBed);
        t_support.text = "";
        texto.text = "";
        //Jugar una pequeña deemo
        speed = 1;
        //->Mostrar los textos
        texto.text = "Grita izquierda o derecha!! Tú decides";
        //////INSTANCIAR LOS PINCHOS Y LAS CAMAS ELÁSTICAS
        this.transform.position = new Vector2(-0.85f, 0.75f);
        elasticBed = Instantiate(trampolin, new Vector2(1.72f, -4.47f), Quaternion.identity);
        spikes = Instantiate(pinchos, new Vector2(-1.0859f, -3.923067f), Quaternion.identity);
        _status = 0;
        _keywordRecognizer.Start();
        while (_status != 1)
        {
            if(_status == -1)
            {
                texto.text = "Tranquilo, intenta otra vez";
                _status = 0;
                speed = 1;
                direction = new Vector2(4, -4);
                assingSprite(this.gameObject, normal);
                this.transform.position = new Vector2(-0.85f, 0.75f);
            }
            yield return new WaitForSeconds(1.5f);
        }
        _keywordRecognizer.Stop();
        blind(this.gameObject);
        blind(spikes);
        blind(elasticBed);
        texto.text = "";
        wdm = Instantiate(welldonemsg, new Vector2(1f, 1f), Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Destroy(wdm);
        //////////////////////////////////////////////////
        /////CAMBIAR DE POSICIÓN
        show(this.gameObject);
        show(spikes);
        show(elasticBed);
        texto.text = "Grita izquierda o derecha!! Tú decides";
        this.transform.position = new Vector2(1.80f, 0.75f);
        elasticBed.transform.position = new Vector2(-1.0859f, -3.923067f);
        spikes.transform.position = new Vector2(1.72f, -4.47f);
        _status = 0;
        _keywordRecognizer.Start();
        while (_status != 1)
        {
            if (_status == -1)
            {
                texto.text = "Tranquilo, intenta otra vez";
                _status = 0;
                speed = 1;
                direction = new Vector2(4, -4);        
                assingSprite(this.gameObject, normal);
                this.transform.position = new Vector2(1.63f, 0.75f);
            }
            yield return new WaitForSeconds(1.5f);
        }
        _keywordRecognizer.Stop();
        Destroy(spikes);
        Destroy(elasticBed);
        blind(this.gameObject);
        texto.text = "";
        wdm = Instantiate(welldonemsg, new Vector2(1f, 1f), Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Destroy(wdm);
        
        
        //Movimientos del cuerpo
        //this.gameObject.SetActive(false);
        t_support.gameObject.SetActive(false);
        texto.text = "Veamos esos movimientos";
        texto.transform.position = new Vector2(350+150f, 300+400f);
        texto.fontSize = 50+40;
        yield return new WaitForSeconds(3f);
        Debug.Log("Procediendo a cambiar escena");
        //Cambiando a tutorial del cuerpo
        SceneManager.LoadScene("Inicio");
        Debug.Log("Cambiando escena");
    }

}
