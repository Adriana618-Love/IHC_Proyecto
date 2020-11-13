using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class staticOrderingMan : MonoBehaviour
{
	[SerializeField]
	private screencapture screencapture_;

	StaticMan staticMan;
	public Text texto;
	public Text jumboTron;
	public GameObject Ballon;
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
		staticMan = new StaticMan(gameObject, screencapture_.objectPose);
		staticMan.makeMove();
	}

	// Update is called once per frame
	void Update()
	{
		if (staticMan.compareMoves(screencapture_.objectPose.moves, screencapture_.objectPose.numMoves) && _tutorialInit)
		{
			//Debug.Log("movimiento hecho");
			texto.text = "Bien Hecho";
			Invoke("Desapear", 1.5f);
			GloboController myScript = Ballon.GetComponent<GloboController>();
			myScript.Elevar();
			//Debug.Log("Se eleva el globo");
			staticMan.makeMove();
			count += 1;
		}
		if (count >= maxMoves)
		{
			_tutorialInit = false;
			//Iniciar una coroutine
			StartCoroutine("TutorialEnd");
		}
	}

	private IEnumerator TutorialEnd()
	{
		///////Descativamos todos los objetos
		Ballon.SetActive(false);
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


public class StaticMan
{
	public ObjectPose staticMan;
	public GameObject currentGameObject;
	public ObjectPose objectPose;
	private screencapture screencapture_;
	public string move;

	public GameObject[] lines;
	public Color colorDefault = Color.white;
	public Color colorMove = Color.red;

	public GameObject face;
	private int numNexos = 13;
	private int[,] BODY_PARTS_KPT_IDS =
						{{1, 2}, {1, 5}, 
						//brazos
						{2, 3}, {3, 4},
						{5, 6}, {6, 7},
						//tronco 1
						{1, 8}, 
						//pierna izquierda
						{8, 9}, {9, 10}, 
						//tronco 2
						{1, 11},
						//pierna derecha
						{11, 12},{12, 13},
						//cuello
						{1, 0}, 
						 /*{0, 14}, {14, 16}, {0, 15}, {15, 17}, {2, 16}, {5, 17}*/
						 };


	private point[] keyPoints = {
		new point(0.0f,1.5f),//nose(0)
        new point(0.0f,1f),//cuello(1)
        new point(-1.0f,0.5f),//hombroD(2)
        new point(-1.25f,0.0f),//codoD(3)
        new point(-1.0f,-0.5f),//manoD(4)
        new point(1.0f,0.5f),//hombroI(5)
        new point(1.25f,0.0f),//codoI(6)
        new point(1.0f,-0.5f),//manoI(7)
        new point(-0.2f,-0.5f),//caderaD(8)
        new point(-0.6f,-1.2f),//rodillaD(9)
        new point(-0.5f,-2.0f),//pieD(10)
        new point(0.2f,-0.5f),//caderaI(11)
        new point(0.6f,-1.2f),//rodillaI(12)
        new point(0.5f,-2.0f),//pieI(13)
    };


	public StaticMan(GameObject currentGameObject_, ObjectPose objectPose_)
	{
		currentGameObject = currentGameObject_;
		objectPose = objectPose_;
		lines = new GameObject[numNexos];

		//inicializar lineas
		Vector3 iniVector = currentGameObject.transform.position;
		for (int i = 0; i < numNexos; ++i)
		{
			lines[i] = DrawLine(iniVector, iniVector, colorDefault, 0.2f);
		}

		//dibujar lineas
		drawPoints();
	}

	GameObject DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
	{
		GameObject myLine = new GameObject("DragLine", typeof(LineRenderer));
		myLine.transform.parent = currentGameObject.transform;
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		/*lineRenderer.startWidth = 0.01f;
		lineRenderer.endWidth = 0.01f;*/

		lr.SetWidth(0.06f, 0.06f);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);

		lr.material = new Material(Shader.Find("Sprites/Default"));
		lr.SetColors(color, color);

		return myLine;
	}

	GameObject drawCircle(Vector3 start, float radius, Color color, float duration = 0.2f)
	{
		GameObject.Destroy(face);
		GameObject myLine = new GameObject("DragLine", typeof(LineRenderer));
		myLine.transform.parent = currentGameObject.transform;

		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Sprites/Default"));
		lr.SetColors(color, color);
		lr.SetWidth(0.06f, 0.06f);

		//Constants para dibujar el circulo
		float Theta = 0f;
		float ThetaScale = 0.01f;
		int Size = (int)((1f / ThetaScale) + 1f);

		lr.SetVertexCount(Size);
		for (int i = 0; i < Size; i++)
		{
			Theta += (2.0f * Mathf.PI * ThetaScale);
			float x = radius * Mathf.Cos(Theta);
			float y = radius * Mathf.Sin(Theta);
			lr.SetPosition(i, new Vector3(x, y, 0) + start);
		}
		face = myLine;
		return myLine;
	}

	public void drawPoints()
	{
		point keyPoint_a;
		point keyPoint_b;
		Vector3 punto_a;
		Vector3 punto_b;

		float scaleImage = 3.0f;
		//drawPoints
		for (int i = 0; i < numNexos; ++i)
		{
			keyPoint_a = keyPoints[BODY_PARTS_KPT_IDS[i, 0]];
			keyPoint_b = keyPoints[BODY_PARTS_KPT_IDS[i, 1]];
			LineRenderer lr = lines[i].GetComponent<LineRenderer>();

			punto_a = new Vector3(keyPoint_a.x / scaleImage, keyPoint_a.y / scaleImage, 0.0f) + currentGameObject.transform.position;
			punto_b = new Vector3(keyPoint_b.x / scaleImage, keyPoint_b.y / scaleImage, 0.0f) + currentGameObject.transform.position;
			lr.SetPosition(0, punto_a);
			lr.SetPosition(1, punto_b);
			lr.SetColors(colorDefault, colorDefault);
		}

		//drawFace
		Vector3 iniVector = currentGameObject.transform.position;
		keyPoint_a = keyPoints[0];
		keyPoint_b = keyPoints[1];

		float radius = (keyPoint_a.y - keyPoint_b.y) / scaleImage;
		punto_a = new Vector3(keyPoint_a.x / scaleImage, keyPoint_a.y / scaleImage + radius, 0.0f) + iniVector;

		//Debug.Log("radio: "+radius);
		drawCircle(punto_a, radius, colorDefault, 0.2f);

	}

	public void unpaintLines()
	{
		for (int i = 0; i < numNexos; i++)
		{
			LineRenderer lr = lines[i].GetComponent<LineRenderer>();
			lr.SetColors(colorDefault, colorDefault);
		}
	}

	public void paintLine(int index)
	{
		LineRenderer lr = lines[index].GetComponent<LineRenderer>();
		lr.SetColors(colorMove, colorMove);
	}

	public bool compareMoves(string[] moves, int numMoves)
	{

		for (int i = 0; i < numMoves; i++)
		{
			Debug.Log(moves[i] + " - " + move);
			if (moves[i] == move)
			{
				return true;
			}

		}
		return false;
	}


	public void makeMove()
	{
		unpaintLines();

		System.Random random = new System.Random();
		int num = random.Next(0, 4);

		//=============Paint Moves (los movimientos izquierda y derecha estan invertidos)
		if (num == 0)
		{//BI
			paintLine(2);
			paintLine(3);
			move = "BI";
		}
		else if (num == 1)
		{//BD
			paintLine(4);
			paintLine(5);
			move = "BD";
		}
		else if (num == 2)
		{//PI
			paintLine(7);
			paintLine(8);
			move = "PI";
		}
		else if (num == 3)
		{//PD
			paintLine(10);
			paintLine(11);
			move = "PD";
		}
	}

	public void Desactivate()
	{
		/////////////DESACTIVAR EL OBJETO
	}
}