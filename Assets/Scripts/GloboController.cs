using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;

public class GloboController : MonoBehaviour
{
    public float speed;
    public Vector2 direction;
    public Sprite explosion;
    public GameObject pausa_object;
    public float minY;
    public Vector2 initPos;

    public float static_speed;

    [SerializeField]
    public screencapture screen_capture;

    public AudioSource[] sounds;
    public AudioSource trampolinAudio;
    public AudioSource estallandoAudio;
    private Animator animator;

    private int _status; /*-1 => Exploto, 0 => Nada, 1 => Rebote*/

    private KeywordRecognizer _keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    // Start is called before the first frame update

    private void Awake()
    {
        sounds = GetComponents<AudioSource>();
        estallandoAudio = sounds[0];
        trampolinAudio = sounds[1];
        animator = this.GetComponent<Animator>();
        static_speed = Configs.configuraciones.speed_globo;
    }

    void Start()
    {
        initPos = this.transform.position;
        direction = new Vector2(4, -4);
        speed = static_speed;

        actions.Add("izquierda", Left);
        actions.Add("derecha", Right);

        actions.Add("pausa", Stop);
        actions.Add("menu", Menu);

        _keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        _keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        _keywordRecognizer.Start();
    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void Left()
    {
        transform.Translate(-2, 0, 0);
    }

    private void Right()
    {
        transform.Translate(2, 0, 0);
    }

    public void Stop()
    {

        if (Time.timeScale == 1)
        {    //si la velocidad es 1
            pausa_object.transform.position = new Vector2(0f, 0f);
            Time.timeScale = 0;     //que la velocidad del juego sea 0
        }
        else if (Time.timeScale == 0)
        {   // si la velocidad es 0
            pausa_object.transform.position = new Vector2(-500f, -500f);
            Time.timeScale = 1;     // que la velocidad del juego regrese a 1
        }
    }

    public void Menu()
    {
        //screen_capture.cerrarSockets();
        SceneManager.LoadScene("menuScene");
    }

    public IEnumerator Elevate()
    {
        speed = static_speed;
        direction.y = 4;
        yield return new WaitForSeconds(3f);
        direction.y = -4;
    }
    public IEnumerator ElevateSimple()
    {
        if (this.transform.position.y < 0)
        {
            speed = static_speed;
            direction.y = 4;
            while(this.transform.position.y < -0.2f)
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
        else
        {
            speed = static_speed;
            direction.y = -4;
            while (this.transform.position.y > 0.2f)
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
        speed = 0f;
    }

    public void ES()
    {
        StartCoroutine("ElevateSimple");
    }

    public void Elevar()
    {
       StartCoroutine("Elevate");
    }
    
    public void SetInit()
    {
        this.transform.position = initPos;
        speed = static_speed;
        direction.y = -4;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Spikes(Clone)")
        {
            Debug.Log("Explosion");
            direction.y = direction.x = 0;
            animator.SetTrigger("pum");
            estallandoAudio.Play();
            Invoke("SetInit", 1);
            _status = -1;
        }
        else if (collision.gameObject.name == "Trampolin(Clone)")
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
        if (transform.position.y <= minY) speed = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Left();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Right();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Elevar();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Menu();
        }
    }
}
