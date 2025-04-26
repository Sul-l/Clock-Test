using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTag : MonoBehaviour
{

    Vector2 resolution, resolutionInWorldUnits = new Vector2 (38.32f, 21.6f);  

    void Start()
    {
        resolution = new Vector2 (Screen.width, Screen.height);
    }

    void Update()
    {
        FollowMouse();   
    }

    private void FollowMouse()
    {
        transform.position = Input.mousePosition/resolution * resolutionInWorldUnits; 
    }
    //Width: 19.16 * 2
    //height: 10.8 * 2
}