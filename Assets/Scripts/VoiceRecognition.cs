using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceRecognition : MonoBehaviour
{

    private KeywordRecognizer _keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    // Start is called before the first frame update
    void Start()
    {

        actions.Add("izquierda", Left);
        actions.Add("derecha", Right);
        actions.Add("arriba", Up);
        actions.Add("abajo", Down);

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
        SpriteRenderer my_sprite = GetComponent<SpriteRenderer>();
        my_sprite.color = Color.red;
    }

    private void Right()
    {
        transform.Translate(2, 0, 0);
        SpriteRenderer my_sprite = GetComponent<SpriteRenderer>();
        my_sprite.color = Color.blue;
    }

    private void Up()
    {
        transform.Translate(0, 2, 0);
        SpriteRenderer my_sprite = GetComponent<SpriteRenderer>();
        my_sprite.color = Color.yellow;
    }

    private void Down()
    {
        transform.Translate(0, -2, 0);
        SpriteRenderer my_sprite = GetComponent<SpriteRenderer>();
        my_sprite.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
