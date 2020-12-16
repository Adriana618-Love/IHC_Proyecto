using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class staticOrderingManVent : MonoBehaviour
{
	[SerializeField]
	public screencapture screencapture_;

	StaticMan staticMan;
	public Text texto;
	public Text jumboTron;
	public GameObject Ventilador;
	public int maxMoves;
	public int count;
	private bool _tutorialInit;
	public GameObject welldonemsg;

	// Start is called before the first frame update
	void Start()
	{
		count = 0;
		_tutorialInit = true;
		//ocultar gameObject
		gameObject.GetComponent<Renderer>().enabled = false;

		//inicializar staticMan
		//staticMan = new StaticMan(gameObject, screencapture_.objectPose);
		//staticMan.makeMove();

		StartCoroutine("DetectarVentilador");
	}

	// Update is called once per frame
	void Update()
	{
		if (_tutorialInit)
		{
			
			
		}
		if (count >= maxMoves)
		{
			_tutorialInit = false;
			//Iniciar una coroutine
			StartCoroutine("TutorialEnd");
		}
	}

	private IEnumerator DetectarVentilador()
	{
		VentiladorController ventiladorController = Ventilador.GetComponent<VentiladorController>();
		Debug.Log("----------------");
		Debug.Log(screencapture_.objectPose.moves);
		Debug.Log("==========================");
		ventiladorController.DetectarMovimiento(screencapture_.objectPose.moves, screencapture_.objectPose.numMoves);
		yield return new WaitForSeconds(0.5f);
		StartCoroutine("DetectarVentilador");
	}

	private IEnumerator TutorialEnd()
	{
		///////Descativamos todos los objetos
		Ventilador.SetActive(false);
		staticMan.Desactivate();
		/////////////////////////////////////
		///////Finalizamos el tutorial
		jumboTron.text = "";
		GameObject wdm = Instantiate(welldonemsg, new Vector2(1f, 1f), Quaternion.identity);
		yield return new WaitForSeconds(1f);
		Destroy(wdm);
		jumboTron.text = "Felicidades, has terminado el Tutorial";
		jumboTron.transform.position = new Vector2(120f, 180f);
		yield return new WaitForSeconds(4f);
		SceneManager.LoadScene("menuScene");
	}
	void Desapear()
	{
		texto.text = "";
	}
    public void OnApplicationQuit()
    {

	}
}
