using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour
{
    public float Left;
    public float Right;
    public float Top;
    public float Down;
    public void GetDims()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        width = width / 2;
        float minX = -width;
        float maxX = width;
        //Debug.Log(minX.ToString() + " " + maxX.ToString());
        var lowerLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        var upperRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Left = lowerLeft.x;
        Right = upperRight.x ;
        Top = upperRight.y ;
        Down = lowerLeft.y ;
        float minY = Left / 2;
        float maxY = -Left / 2;

        //Debug.Log(minY.ToString() + " " + maxY.ToString());
        //Debug.Log(Left.ToString() + " " + Right.ToString() + " " +Top.ToString() + " " + Bottom.ToString());
    }
    
    public bool isBoundary(Vector3 pos, float padding=0)
    {
        if(pos.x < (Right+padding) && pos.x > (Left-padding) && pos.y > (Down-padding) && pos.y < (Top+padding))
        {
            return true;
        }
        return false;
    }

}
