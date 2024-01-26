using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragToMove : MonoBehaviour
{

    private Touch touch;
    private float speedModifier; // controls how fast the model will move

    void Start()
    {
        speedModifier = 0.001f;
    }

    void Update()
    {
        if (Input.touchCount > 0) // when finger is touching the screen
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved) // when move gesture is performed
            {
                //change the position of object in each frame, assign new vector 3 values
                transform.position = new Vector3(
                    transform.position.x + touch.deltaPosition.x * speedModifier,
                    transform.position.y,
                    transform.position.z + touch.deltaPosition.y * speedModifier
                    );
                /* original x value of object + how much our finger has moved along the x * speed,
                 * keep original y,
                 * same as first, except for z value of object and y value of finget
                 */
            }
        }
        
    }
}
