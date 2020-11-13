using UnityEngine;
using System.Collections;
using System.IO;

using System.Threading;
using System.Net.Sockets;
using System;
using System.Text;

public class serverManager_vent : MonoBehaviour
{
    internal Boolean socketReady = false;

	public TcpClient mySocket;
	NetworkStream theStream;
	StreamWriter theWriter;
	StreamReader theReader;
	String Host = "127.0.0.1";
	Int32 Port = 2000;
	ReadServer_vent ReadServer_vent;

	private string mensaje = "";

	//GloboController
	public GameObject GloboController_;
    private GloboController globoController;


	// Take a shot immediately
	public void Start()
	{
		//GloboController
        globoController = GloboController_.GetComponent<GloboController>();

		StartCoroutine("intentarConectar");
	}

	public void Update()
	{	
		if (socketReady)
		{
			detectMove(mensaje);
			mensaje = "";
		}
	}

	
    IEnumerator intentarConectar()
    {
		Debug.Log("Intentando conectar al serverManager_vent..");
        //intentar conectar con el server
		setupSocket ();
		if(socketReady){
			Debug.Log("Conectado al serverManager_vent..");
			//enviar mensaje para confirmar la conexión con el server
			/*theWriter.Write('Y');
			theWriter.Flush();*/
			
			//inicializar thread de lectura
			ReadServer_vent = new ReadServer_vent();
			ReadServer_vent.readServer(mySocket, theStream, this);
		}
		else{
			yield return new WaitForSeconds(5);
			StartCoroutine("intentarConectar");
		}
        
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
			ReadServer_vent.close();
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
			if (mensaje[0] == 'G'){
				detectVentiladorMove(mensaje);
			}
			else if (mensaje[0] == 'V'){
				detectVentiladorMove(mensaje);
			}
		}
	}
	

	public void detectVentiladorMove(string mensaje){
		if (mensaje[1] == 'G'){
			Debug.Log("Ventilador giro horario");
			globoController.Left_();
		}
		else if (mensaje[1] == 'H'){
			Debug.Log("Ventilador giro antihorario");
			globoController.Right_();
		}
	}
}


//Class to handle each client request separatly
public class ReadServer_vent
{
	TcpClient mySocket;
	NetworkStream theStream;
	Thread ctThread;
	public bool socketReady;
 	serverManager_vent serverManager_vent_;

	public void readServer(TcpClient mySocket_, NetworkStream theStream_, serverManager_vent serverManager_vent__)
	{ 
		mySocket = mySocket_;
		theStream = theStream_;
		socketReady = true;
		serverManager_vent_ = serverManager_vent__;
		ctThread = new Thread(read);
		ctThread.Start();
	}
	private void read()
	{
		byte[] myReadBuffer = new byte[2];
		string dataFromServer = null;
		int sizeOfMessage = 2;
		while (true)
		{
			try
			{
				if(!socketReady){
					break;
				}

				theStream.Read(myReadBuffer, 0, sizeOfMessage);
				dataFromServer = System.Text.Encoding.ASCII.GetString(myReadBuffer);
				//Debug.Log(" >> From server-" + dataFromServer);

				//serverManager_.detectMove(dataFromServer);
				serverManager_vent_.setMensaje(dataFromServer);
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