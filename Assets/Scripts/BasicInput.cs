using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInput : MonoBehaviour
{

    public float cameraSpeed = 5f;

    private bool isMoving = false;

    private Vector3 lastMousePosition;

    // Update is called once per frame
    void Update()
    {
        //only allow camera movement if the player is playing the game
        if (GameMechanics.playingGame)
        {
            //if middle mouse button is pressed then allow movement;
            if (Input.GetMouseButtonDown(2))
            {
                isMoving = true;
                lastMousePosition = Input.mousePosition;
            }

            //if middle mouse button is released then disallow movement
            if (Input.GetMouseButtonUp(2))
            {
                isMoving = false;
            }

            //if middle mouse button is held down, then we can move the camera
            if (isMoving)
            {
                Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;
                Vector3 moveDirection = new Vector3(deltaMousePosition.x, deltaMousePosition.y, 0f);

                //adjust camera posn
                transform.Translate(moveDirection * cameraSpeed * Time.deltaTime);

                //update last mouse psn
                lastMousePosition = Input.mousePosition;
            }

            //allow scroll wheel to adjust z axis 
            float ScrollInput = Input.GetAxis("Mouse ScrollWheel");
            transform.Translate(Vector3.forward * ScrollInput * cameraSpeed * 100 * Time.deltaTime);

            if (Input.GetKey(KeyCode.R))
            {
                transform.position = (new Vector3(4, 4, -10));
            }
        }
    }
}