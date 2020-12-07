using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomManager : MonoBehaviour
{
    public GameObject Tutorial;
    public GameObject MiniGame;
    public GameObject MainCamera;

    public GameObject B1;
    public GameObject B2;

    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartTutorial()
    {
        MainCamera.transform.position = new Vector3(30, 0,-10);
        Tutorial.SetActive(true);
        B1.SetActive(false);
        B2.SetActive(false);
    }

    public void StartMiniGame()
    {
        text.text = "Conectandose a una sala...";
        MainCamera.transform.position = new Vector3(0, 0,-10);
        MiniGame.SetActive(true);
        B1.SetActive(false);
        B2.SetActive(false);
    }
}
