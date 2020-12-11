using UnityEngine;
using System.Collections;
using System.IO;

using System.Net.Sockets;
using System;
using System.Text;

public class screencapture : MonoBehaviour
{
	internal Boolean socketReady = false;

	public TcpClient mySocket;
	NetworkStream theStream;
	StreamWriter theWriter;
	StreamReader theReader;
	String Host = "127.0.0.1";
	//String Host = "3.236.120.217";
	Int32 Port = 1234;
	public ObjectPose objectPose;
	int numImagesSent = 0;

	WebCamTexture webCamTexture;

	// Take a shot immediately
	public void Start()
	{
		//ocultar gameObject
		gameObject.GetComponent<Renderer>().enabled = false;

		//setear objectPose
		objectPose = new ObjectPose(gameObject);

		//setear webcam
		/*webCamTexture = new WebCamTexture();
		Debug.Log(webCamTexture.deviceName);

		Renderer renderer = GetComponent<Renderer>();
		renderer.material.mainTexture = webCamTexture;
		webCamTexture.Play();*/


		//StartCoroutine(write ()); 
		/*setupSocket();
		theWriter.Write('Y');
		theWriter.Flush();*/
		StartCoroutine("intentarConectar");
	}

	public void Update()
	{
		//StartCoroutine(write ());

		if (socketReady)
		{
			//cuando el servidor no se encuentra corriendo
			//Debug.Log("Juega con tus movimientos!!!...");

			readSocket();
		}
		/*else{
			//cuando el servidor no se encuentra corriendo
			Debug.Log("Usa las teclas!!!...");
		}*/
	}

	
    IEnumerator intentarConectar()
    {
		Debug.Log("Intentando conectar al server..");
        //intentar conectar con el server
		setupSocket ();
		if(socketReady){
			Debug.Log("Conectado al server..");
			//enviar mensaje para confirmar la conexión con el server
			theWriter.Write('Y');
			theWriter.Flush();
		}
		else{
			yield return new WaitForSeconds(5);
			StartCoroutine("intentarConectar");
		}
        
    }

	void OnApplicationQuit()
	{
		if(socketReady){
			Debug.Log("Cerrando connexión sockets");
			//enviar mensaje para cerrar la conexión
			theWriter.Write('Q');
			theWriter.Flush();

			/*byte[] myReadBuffer = new byte[1024];
			theStream.Read(myReadBuffer, 0, 2);
			string mensaje = Encoding.ASCII.GetString(myReadBuffer, 0, 2);
			Debug.Log("--________---");
			Debug.Log(mensaje);

			theStream.Close();
			mySocket.Close();*/
			socketReady = false;
		}
	}

	public void cerrarSockets()
    {
		if(socketReady){
			Debug.Log("Cerrando connexión sockets");
			//enviar mensaje para cerrar la conexión
			theWriter.Write('Q');
			theWriter.Flush();
			theStream.Close();
			mySocket.Close();
			socketReady = false;
		}
	}

	public void setupSocket()
	{
		try
		{
			mySocket = new TcpClient(Host, Port);

			theStream = mySocket.GetStream();
			theWriter = new StreamWriter(theStream);
			theReader = new StreamReader(theStream);
			socketReady = true;
		}
		catch (Exception e)
		{
			Debug.Log("Socket error: " + e);
		}
	}

	public IEnumerator write()
	{
		yield return new WaitForEndOfFrame();

		int width = webCamTexture.width;
		int height = webCamTexture.height;

		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.SetPixels(webCamTexture.GetPixels());
		tex.Apply();

		byte[] bytes = tex.EncodeToPNG();
		String rpcText = System.Convert.ToBase64String(bytes);
		//Debug.Log("------------------------------------------------------");
		//Debug.Log(rpcText);


		//-----------------
		int sizeBuffer = 1024;
		int BUFFER_SIZE = 1024;

		//rpcText = rpcText + "$";


		//int k = (int)Math.Ceiling((rpcText.Length) / 1024.0);
		//int offset = k*BUFFER_SIZE;

		//while(rpcText.Length < offset)
		//	rpcText = rpcText + "0";


		string lengthFixed = rpcText.Length.ToString();
		while (lengthFixed.Length < 10)
			lengthFixed = "0" + lengthFixed;
		//lengthFixed += "@";
		//Debug.Log ("message Lenght:"+lengthFixed);

		theWriter.Write(lengthFixed);
		theWriter.Flush();

		//Debug.Log("----k: "+k);
		//Debug.Log(rpcText);
		//Debug.Log("--------------------size---------------------" + rpcText.Length + ", k:"+k );
		/*
		for (int i = 0; i < k; i++) {
			String temp=rpcText.Substring(i*BUFFER_SIZE,BUFFER_SIZE);
			//String sizeI = i.ToString();
			//while(sizeI.Length < 3)
			//	sizeI = "0" + sizeI;

			//temp = sizeI + temp;

			//Debug.Log("------------------------------------------------------" + sizeI );
			//Debug.Log(temp);

			theWriter.Write(temp);
			theWriter.Flush();
		}

		Debug.Log("------------------numImgSent:---------"+numImagesSent);
		++numImagesSent;*/


		int k = (rpcText.Length) / 1024;
		int j = (rpcText.Length) % 1024;
		int i = 0;

		for (; i < k; i++)
		{
			String temp = rpcText.Substring(i * 1024, 1024);
			//Debug.Log("------------------------------------------------------"+i.ToString() );
			//Debug.Log(rpcText);
			theWriter.Write(temp);
			theWriter.Flush();
		}

		String temp2 = rpcText.Substring(i * 1024, j);
		//Debug.Log("___________________ k = " + k.ToString()+ ", j = " + j.ToString());
		theWriter.Write(temp2);
		theWriter.Flush();


		yield return new WaitForSeconds(0.0f);

	}


	public String readSocket()
	{
		if (!socketReady)
		{
			Debug.Log("socket no esta listo");
			return "";
		}
		if (theStream.CanRead)
		{
			int sizeOfMessage = 1024;
			byte[] myReadBuffer = new byte[1024];
			StringBuilder completeMessage = new StringBuilder();
			int numberOfBytesRead = 0;

			// Incoming message may be larger than the buffer size.
			do
			{
				numberOfBytesRead = theStream.Read(myReadBuffer, 0, sizeOfMessage);
				completeMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
				objectPose.setAtributes(completeMessage.ToString());
			}
			while (theStream.DataAvailable);

			//enviar mensaje para confirmar la recepcion del mensaje del server
			theWriter.Write('N');
			theWriter.Flush();
		}
		else
		{
			Debug.Log("Sorry.  You cannot read from this NetworkStream.");
		}

		return "";
	}

}

public struct point
{
	public float x, y;
	public point(float x_, float y_)
	{
		x = x_;
		y = y_;
	}
}

public class ObjectPose
{
	public ObjectPose staticMan;
	public GameObject currentGameObject;

	public string[] moves;
	public int numMoves;
	public point[] keyPoints;
	public const int numKeyPoints = 18;
	public GameObject[] lines;
	public Color colorDefault = Color.white;
	public Color colorMove = Color.red;

	public GameObject face;
	public int numNexos = 13;
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

	public ObjectPose(GameObject currentGameObject_)
	{
		currentGameObject = currentGameObject_;
		keyPoints = new point[numKeyPoints];
		lines = new GameObject[numNexos];

		//inicializar lineas
		Vector3 iniVector = currentGameObject.transform.position;
		for (int i = 0; i < numNexos; ++i)
		{
			lines[i] = DrawLine(iniVector, iniVector, colorDefault, 0.2f);
		}
	}

	GameObject DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
	{
		GameObject myLine = new GameObject("DragLine", typeof(LineRenderer));
		myLine.transform.parent = currentGameObject.transform;
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		/*lineRenderer.startWidth = 0.01f;
		lineRenderer.endWidth = 0.01f;*/

		lr.SetWidth(0.1f, 0.1f);
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
		lr.SetWidth(0.1f, 0.1f);

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

		//drawPoints
		for (int i = 0; i < numNexos; ++i)
		{
			keyPoint_a = keyPoints[BODY_PARTS_KPT_IDS[i, 0]];
			keyPoint_b = keyPoints[BODY_PARTS_KPT_IDS[i, 1]];
			LineRenderer lr = lines[i].GetComponent<LineRenderer>();

			if (keyPoint_a.x != -1.0f && keyPoint_b.x != -1.0f)
			{
				//mostrar gameObject
				lines[i].GetComponent<Renderer>().enabled = true;

				punto_a = new Vector3(keyPoint_a.x, keyPoint_a.y, 0.0f) + currentGameObject.transform.position;
				punto_b = new Vector3(keyPoint_b.x, keyPoint_b.y, 0.0f) + currentGameObject.transform.position;
				lr.SetPosition(0, punto_a);
				lr.SetPosition(1, punto_b);
				lr.SetColors(colorDefault, colorDefault);
			}
			else
			{
				//ocultar gameObject
				lines[i].GetComponent<Renderer>().enabled = false;
			}
		}

		//Paint Moves
		for (int i = 0; i < numMoves; ++i)
		{
			if (moves[i] == "BD")
			{
				paintLine(2);
				paintLine(3);
			}
			else if (moves[i] == "BI")
			{
				paintLine(4);
				paintLine(5);
			}
			else if (moves[i] == "PD")
			{
				paintLine(7);
				paintLine(8);
			}
			else if (moves[i] == "PI")
			{
				paintLine(10);
				paintLine(11);
			}

		}


		//drawFace
		Vector3 iniVector = currentGameObject.transform.position;
		keyPoint_a = keyPoints[0];
		keyPoint_b = keyPoints[1];

		float radius = (keyPoint_a.y - keyPoint_b.y);
		punto_a = new Vector3(keyPoint_a.x, keyPoint_a.y + radius, 0.0f) + iniVector;

		//Debug.Log("radio: "+radius);
		drawCircle(punto_a, radius, colorDefault, 0.2f);

	}

	public void paintLine(int index)
	{
		LineRenderer lr = lines[index].GetComponent<LineRenderer>();
		lr.SetColors(colorMove, colorMove);
	}

	public void setAtributes(string message)
	{
		float scaleImg = 1.5f * 100f;
		string auxMsg;
		//Debug.Log("Mensaje recibido: ");
		//Debug.Log(message);

		//=====Obterner la cantidad de movimientos y los movimientos
		int iter = 0;
		numMoves = message[0] - '0';
		//Debug.Log( message[0] - '0' );
		++iter;

		moves = new string[numMoves];
		for (int i = 0; i < numMoves; ++i)
		{
			moves[i] = message.Substring(iter, 2);
			iter += 2;
		}
		//debug moves
		/*Debug.Log("==========MOVES========");
		for(int i=0; i<numMoves; ++i){
			Debug.Log("=="+moves[i]);
		}*/

		//=====Obterner los keypoints detectados
		for (int i = 0; i < numKeyPoints; ++i)
		{
			if ((auxMsg = message.Substring(iter, 3)) != "0-1")
			{
				keyPoints[i].x = (float.Parse(auxMsg) * -1 + 250.0f) / scaleImg;
				keyPoints[i].y = (float.Parse(message.Substring(iter + 3, 3)) * -1 + 250f) / scaleImg;
				iter += 6;
			}
			else
			{
				keyPoints[i].x = -1.0f;
				keyPoints[i].y = -1.0f;
			}
		}
		drawPoints();
		//debug moves
		/*Debug.Log("==========KEYPOINTS========");
		for(int i=0; i<numKeyPoints; ++i){
			Debug.Log("x: " + keyPoints[i].x.ToString() + ", y: " + keyPoints[i].y.ToString());
		}*/
	}
}
