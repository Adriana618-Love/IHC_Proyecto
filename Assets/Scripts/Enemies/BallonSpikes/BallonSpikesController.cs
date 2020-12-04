using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallonSpikesController : MonoBehaviour
{
    //server manager
    public serverManager server;

    public int maxBallons;
    public GameObject[] ballons;
    private int _index;
    public GameObject BALLON;
    public Camara cam;
    // Start is called before the first frame update
    void Start()
    {
        server = serverManager._server_;
        cam = new Camara();
        maxBallons = 4;
        ballons = new GameObject[maxBallons];
        _index = 0;
        cam.GetDims();
    }

    void GenerateBallon()
    {
        ballons[_index] = Instantiate(BALLON, new Vector2(Random.Range(cam.Left, cam.Right), Random.Range(cam.Down, cam.Top)), Quaternion.identity);
        //ballons[_index].transform.parent = this.transform;
        _index += 1;
        maxBallons -= 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*if(server.ballonSpikes.Count > 0){
            int positionX = server.ballonSpikes[0].Key;
            int positionY = server.ballonSpikes[0].Value;

            transform.position = new Vector2(positionX, positionY);
            Instantiate(BALLON, transform.position, Quaternion.identity);

            server.ballonSpikes.RemoveAt(0);
        }*/
    }

    public void GenerarBalloonSpike(int positionX, int positionY)
    {
        transform.position = new Vector2(positionX, positionY);
        Instantiate(BALLON, transform.position, Quaternion.identity);
    }
}
