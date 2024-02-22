using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInput : MonoBehaviour
{

    public int direction = 1;

    void Start()
    {
        //Debug.Log($"Hello World!");
    }

    // Update is called once per frame
    void Update()
    {
        /*  Camera Movement options
         *  
         *  `Q` inverts the direction to be traveled
         *  `X`, `Y`, `Z` travels along that direction
         */
        if (Input.GetKeyDown(KeyCode.Q))
        {
            direction *= -1;
        }

        if (Input.GetKey(KeyCode.X))
        {
            transform.Translate(new Vector3(direction, 0, 0));
        }
        
        if (Input.GetKey(KeyCode.Y))
        {
            transform.Translate(new Vector3(0, direction, 0));
        }

        if (Input.GetKey(KeyCode.Z))
        {
            transform.Translate(new Vector3(0, 0, direction));
        }

        //`R` reset's the camera to the starting default position -- dependent on the difficulty selected!

        if (Input.GetKey(KeyCode.R))
        {
            //easy
            transform.position = (new Vector3(4, 4, -10));

            //medium

            //hard

            //custom
        }

        if (Input.GetKey(KeyCode.D))
        {
            //ability to change the difficulty (cycle through 0 -> 4)
            //difficulty = 5;
        }
    }
}