// Title: CameraMovement.cs
// Description: script to move the camera by the character in Mason's Missing Keys
// Instruction Credit: "Mister Taft Creates" @ YouTube
// Author, Adjusting, Commenting: Carter Roberts
// Professor, Class: Omar EL Khatib, Game Programming
// Date Created: 5/6/2024 (MM/DD/YYYY)
// Date Modified: 5/9/2024 (MM/DD/YYYY)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // approx. speed of movement towards target
    public float smoothing;

    // reference to the camera's followed target 
    public Transform target;

    // max and min bounds of the camera's reach
    public Vector2 maxPosition;
    public Vector2 minPosition;

    // FixedUpdate is called a set amount of times every frame
    void FixedUpdate()
    {
            // if the position of the camera is not the same as its target,
        if (transform.position != target.position) 
        {
                // to avoid moving directly onto the z point of the target and rendering the
                // camera useless, a new Vector, targetPosition, is made of target's x and y 
                // position and the z position of the camera.
            Vector3 targetPosition = new Vector3(target.position.x,
                                                 target.position.y,
                                                 transform.position.z);
                // forces x and y of new Vector to be within bounds of reach
            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);

                // interpolates from camera's position to target's position at rate of smoothing
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}
