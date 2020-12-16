using UnityEngine;
using System.Collections;
using System.IO;

using System.Threading;
using System.Net.Sockets;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class serverManager : MonoBehaviour
{
	public static serverManager _server_;
	internal Boolean socketReady = false;

	public TcpClient mySocket;
	NetworkStream theStream;
	StreamWriter theWriter;
	StreamReader theReader;
	//String Host = "26.162.26.142";
	String Host = "127.0.0.1";
	Int32 Port = 2000;
	ReadServer readServer;

	private string mensaje = "";
	public string rol = "";

	//GloboController
	public GameObject GloboController_;
    private GloboSlave globoController;
	public VentiladorSlave ventiladorController;

	public GameObject GameLord;

	//Array de mensajesRecidibos
	public List<string> mensajes = new List<string>();

	//Array de spikes (0->trampolin, 1->spike)
	public List<char> spikes = new List<char>();

	//Array de cometas
	private CometaGenerator cometaGenerator;
	public List<int> cometas = new List<int>();
	public List<int> tipoCometas = new List<int>();

	//array de ballonSpikes
	private BallonSpikesController ballonSpikesController;
	public List<KeyValuePair<int, int>> ballonSpikes = new List<KeyValuePair<int, int>>();

	//conexion de jugadores
	private bool globoConectado = false;
	private bool ventiladorConectado = false;

	//gameobjects de los estados del juego
	private GameObject estadoGlobo;
	private GameObject estadoVentilador;
	private GameObject texto_error_al_conectar;

	private void Awake()
	{
		if (_server_ == null)
		{
			_server_ = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else if (_server_ != this)
		{
			Destroy(this.gameObject);
		}
	}

	public void Contextualizar()
    {
		GameObject Game = GameObject.Find("Game");
		GloboController_ = Game.transform.Find("Balloon").gameObject;
		globoController = GloboController_.GetComponent<GloboSlave>();
		ventiladorController = Game.transform.Find("Ventilador").gameObject.GetComponent<VentiladorSlave>();
		cometaGenerator = Game.transform.Find("CometasGenerator").gameObject.GetComponent<CometaGenerator>();
		ballonSpikesController = Game.transform.Find("BalloonSpikesController").gameObject.GetComponent<BallonSpikesController>();

		estadoGlobo = GameObject.Find("Estado Globo");
		estadoVentilador = GameObject.Find("Estado Ventilador");
		GameObject canvas = GameObject.Find("Canvas");
		texto_error_al_conectar = canvas.transform.Find("texto_error_al_conectar").gameObject;

		GameLord = GameObject.Find("GameLord");
		GameLord.GetComponent<GameLord>().Iniciar();
	}

	public void Start()
	{
		rol = "N";
		//GloboController
		if (GloboController_ != null)
		{
			globoController = GloboController_.GetComponent<GloboSlave>();
		}
		if(GameLord != null)
        {
			Debug.Log("sinGameLord");
        }
		StartCoroutine("intentarConectar");
		StartCoroutine("enviarACK");
	}

	public void Update()
	{	
		if (socketReady)
		{
			if(mensajes.Count > 0){
				detectMove(mensajes[0]);
				mensajes.RemoveAt(0);
			}
		}
	}

	IEnumerator intentarConectar()
    {
		Debug.Log("Intentando conectar al serverManager..");
        //intentar conectar con el server
		setupSocket ();
		if(socketReady){
			Debug.Log("Conectado al serverManager..");
			//enviar mensaje para confirmar la conexión con el server
			/*theWriter.Write('Y');
			theWriter.Flush();*/
			//inicializar thread de lectura
			readServer = new ReadServer();
			readServer.readServer(mySocket, theStream, this);
		}
		else{
			yield return new WaitForSeconds(5);
			StartCoroutine("intentarConectar");
		}
        
    }

	IEnumerator enviarACK()
    {
		if (socketReady)
		{
			write("AC");
		}
		yield return new WaitForSeconds(2);
		StartCoroutine("enviarACK");   
    }

	void OnApplicationQuit()
	{
		cerrarSockets();
	}

	public void cerrarSockets()
    {
		if(socketReady){
			Debug.Log("Cerrando connexión sockets");
			//enviar mensaje para cerrar la conexión
			theWriter.Write("QQ");
			theWriter.Flush();
			theStream.Close();
			mySocket.Close();
			readServer.close();
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

    public void write(string message)
	{
		theWriter.Write(message);
		theWriter.Flush();

	}	

	public void setMensaje(string mensaje_){
		mensaje = mensaje_;
	}

	public void detectMove(string mensaje){
		if(mensaje != ""){
			print("mensaje"+mensaje);
			if(mensaje[0] == 'S'){
				//spikes (0s o 1s)
				detectSpikes(mensaje);
			}
			else if (mensaje[0] == 'C'){
				detectCometas(mensaje);
			}
			else if (mensaje[0] == 'B'){
				detectBalloonSpikes(mensaje);
			}
			else if (mensaje[0] == 'G'){
				Debug.Log("mensaje globo: " + mensaje);
				detectGloboMove(mensaje);
			}
			else if (mensaje[0] == 'V'){
				Debug.Log("mensaje ventilador: " + mensaje);
				detectVentiladorMove(mensaje);
			}
			else if(mensaje[0] == 'H')
            {
				//mensaje para activar la gravedad x3
				activarGravedad();
            }
			else if(mensaje[0] == 'U')
            {
				//mensaje para sumar vidas
				if(mensaje[1] == 'G'){
					agregarVidaGlobo();
				}
				else if(mensaje[1] == 'V'){
					agregarVidaVentilador();
				}
            }
			else if(mensaje[0] == 'A')
            {
				setRol(mensaje);
            }
			else if(mensaje[0] == 'E')
            {
				Debug.Log("Empezar");
				globoConectado = true;
				ventiladorConectado = true;

				redirectScene(mensaje);
            }

		}
	}

	public void activarGravedad()
	{
		
	}

	public void agregarVidaGlobo()
	{
		globoController.AddVIDA();
	}

	public void agregarVidaVentilador()
	{
		ventiladorController.AddEnergy();
	}

	public void redirectScene(string mensaje)
	{
		//roles: G o V
		string sceneName = "";

        if (rol == "G")
        {
			sceneName = "Main";
        }
        else
        {
			sceneName = "Main_vent";
        }

		SceneManager.LoadScene(sceneName);
	}


	public void setRol(string mensaje){
		//roles: G o V
		rol = Char.ToString(mensaje[1]);
		Debug.Log(rol);
		/*if(mensaje[0] ==  'G'){
			//si es el globo, enviar seteo de dificultad al server

		}*/
	}

	public void detectGloboMove(string mensaje){
		if (mensaje[1] == 'L'){
			//Debug.Log("Globo Izquierda");
			globoController.Left_();
		}
		else if (mensaje[1] == 'R'){
			//Debug.Log("Globo Derecha");
			globoController.Right_();
		}
		else if (mensaje[1] == 'A'){
			//Debug.Log("Globo Arriba");
			globoController.Elevar_();
		}
		else if (mensaje[1] == 'S'){
			Debug.Log("Globo Conectado");
			//se debería pintar de verde el conectado
			globoConectado = true;
		}
		else if (mensaje[1] == 'D'){
			Debug.Log("Globo Desconectado");
			//se debería pintar de rojo el conectado
			globoConectado = false;

			//setear sprite de color rojo 
			GameObject redCircle = estadoGlobo.transform.Find("RedCircle").gameObject;
			GameObject greenCircle = estadoGlobo.transform.Find("GreenCIrcle").gameObject;

			redCircle.SetActive(true); 
			greenCircle.SetActive(false); 

			StartCoroutine("salirMenu");
		}
	}

	public void detectVentiladorMove(string mensaje){
		if (mensaje[1] == 'G'){
			//Debug.Log("Ventilador giro antihorario");
			ventiladorController.GirarAntihorario_();
		}
		else if (mensaje[1] == 'H'){
			//Debug.Log("Ventilador giro horario");
			ventiladorController.GirarHorario_();
		}
		else if (mensaje[1] == 'A'){
			ventiladorController.Elevar_();
		}
		else if (mensaje[1] == 'B'){
			ventiladorController.Bajar_();
		}
		else if (mensaje[1] == 'L'){
			ventiladorController.Left_();
		}
		else if (mensaje[1] == 'R'){
			ventiladorController.Right_();
		}
		else if (mensaje[1] == 'S'){
			Debug.Log("Ventilador Conectado");
			ventiladorConectado = true;
		}
		else if (mensaje[1] == 'D'){
			Debug.Log("Ventilador Desconectado");
			ventiladorConectado = false;

			//setear sprite de color rojo 
			GameObject redCircle = estadoVentilador.transform.Find("RedCircle").gameObject;
			GameObject greenCircle = estadoVentilador.transform.Find("GreenCIrcle").gameObject;
			///Ocultando la barra y ventilador estático
			GameObject Canvas = GameObject.Find("Canvas");
			Canvas.transform.Find("HealthBar").gameObject.SetActive(false);
			Canvas.transform.Find("vent_vidas").gameObject.SetActive(false);
			

			redCircle.SetActive(true); 
			greenCircle.SetActive(false);

			GameObject Game = GameObject.Find("Game");
			Game.transform.Find("Ventilador").gameObject.SetActive(false); 
			//StartCoroutine("salirMenu");
		}
	}

	public void detectSpikes(string mensaje){
		for(int i = 1; i<mensaje.Length; ++i){
			spikes.Add(mensaje[i]);
		}
	}

	public void detectCometas(string mensaje){
		string tipoCometa = mensaje.Substring(1, 1);
		tipoCometas.Add( Int16.Parse(tipoCometa));

		string strNum = mensaje.Substring(2, 2);
		int num = Int16.Parse(strNum);
		//normalizar y agregar
		cometas.Add( num - 18);

		cometaGenerator.GenerarCometa(Int16.Parse(tipoCometa), num-18);
	}

	public void detectBalloonSpikes(string mensaje){
		string posX = mensaje.Substring(1, 2);
		string posY = mensaje.Substring(3, 2);
		var pair = new KeyValuePair<int, int>( Int16.Parse(posX) - 18,  Int16.Parse(posY) - 8);
		ballonSpikes.Add( pair );

		ballonSpikesController.GenerarBalloonSpike(Int16.Parse(posX) - 18, Int16.Parse(posY) - 8);
	}


	IEnumerator salirMenu()
    {
		yield return new WaitForSeconds(1);
		texto_error_al_conectar.SetActive(true);
		yield return new WaitForSeconds(5);
		write("QQ");
		SceneManager.LoadScene("menuScene");
    }
	
}


//Class to handle each client request separatly
public class ReadServer
{
	TcpClient mySocket;
	NetworkStream theStream;
	Thread ctThread;
	public bool socketReady;
 	serverManager serverManager_;

	public void readServer(TcpClient mySocket_, NetworkStream theStream_, serverManager serverManager__)
	{ 
		mySocket = mySocket_;
		theStream = theStream_;
		socketReady = true;
		serverManager_ = serverManager__;
		ctThread = new Thread(read);
		ctThread.Start();
	}
	private void read()
	{
		byte[] myReadBuffer = new byte[26];
		string dataFromServer = null;
		int sizeOfMessage = 26;
		while (true)
		{
			try
			{
				if(!socketReady){
					break;
				}

				theStream.Read(myReadBuffer, 0, sizeOfMessage);
				dataFromServer = System.Text.Encoding.ASCII.GetString(myReadBuffer);
				
				serverManager_.mensajes.Add(dataFromServer);

				//vaciar buffer
				Array.Clear(myReadBuffer, 0, 26);
			}
			catch (Exception ex)
			{
				Debug.Log(" Error >> " + ex.ToString());
			}
		}

		close();
	}

	

	public void close(){
		socketReady = false;
		ctThread.Interrupt();
		/*if(!ctThread.Join(2000)) { // or an agreed resonable time
			ctThread.Abort();
		}*/
	}
} 