using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BackgroundController : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect;
    private Transform camTransform;

    public float trainSpeed = 5f;
    private float elapsedTime = 0f;
    private bool isRunning = true;


    public static event Action OnCutsceneEnd;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("Camera isn't assigned");
            return;
        }
        camTransform = cam.transform; 
        startPos = transform.position.x;

    }

    void Update()
    {
        
        if (cam != null)
        {
            
            float distance = cam.transform.position.x * parallaxEffect;

            
            transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
        }

        {
            // Move layers based on time and parallax effect
            float distance = Time.time * trainSpeed * parallaxEffect;
            transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

            // Update timer
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 7f) // After 4 seconds
            {
                isRunning = false; // Stop movement
                OnCutsceneEnd?.Invoke(); // Trigger the scene change event
            }
        }
    }

}
