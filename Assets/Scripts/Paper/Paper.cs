using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : MonoBehaviour
{
    // Start is called before the first frame update
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    private float waitingTime;
    void Start()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        width = width / 2;
        minX = -width + 1;
        maxX = width - 1;
        //Debug.Log(minX.ToString() + " " + maxX.ToString());
        var lowerLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        var upperRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float Left = lowerLeft.x + 5f;
        float Right = upperRight.x - 5f;
        float Top = upperRight.y - 5f;
        float Bottom = lowerLeft.y + 5f;
        //Debug.Log(Left.ToString()+" " + Right.ToString()+" " + Top.ToString() +" "+ Bottom.ToString());
        minY = Left/2;
        maxY = -Left/2;
        Debug.Log(minY.ToString() + " " + maxY.ToString());
        waitingTime = 2f;
}

    private void blind(GameObject obj)
    {
        obj.GetComponent<Renderer>().enabled = false;
    }

    private void show(GameObject obj)
    {
        obj.GetComponent<Renderer>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Cogiste un paper");
        blind(this.gameObject);
        StartCoroutine("changePosition");
    }

    IEnumerator changePosition()
    {
        yield return new WaitForSeconds(waitingTime);
        transform.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        show(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
