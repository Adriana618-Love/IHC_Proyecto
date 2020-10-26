using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cometa : MonoBehaviour
{
    private float min_Y;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        var lowerLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        var upperRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float Left = lowerLeft.x + 5f;
        float Right = upperRight.x - 5f;
        float Top = upperRight.y - 5f;
        float Bottom = lowerLeft.y + 5f;
        //Debug.Log(Left.ToString()+" " + Right.ToString()+" " + Top.ToString() +" "+ Bottom.ToString());
        min_Y = Left;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= min_Y)
        {
            Destroy(this.gameObject);
        }
        transform.Translate(0,-(speed * Time.deltaTime), 0);
    }
}
