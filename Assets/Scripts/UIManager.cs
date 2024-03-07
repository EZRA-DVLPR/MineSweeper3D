using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public delegate void ChangeGridSize(int newLength, int newWidth);
    public static event ChangeGridSize OnChangeGridSize;

    public void ButtonClicked(int id)
    {
        Debug.Log($"You clicked a menu button with id: {id}");
        
        switch(id)
        {
            //easy
            case 0:
                OnChangeGridSize?.Invoke(5, 5);
                //OnChangeSize?.Invoke(9, 9);
                break;

            //medium
            case 1:
                OnChangeGridSize?.Invoke(16, 16);
                break;

            //hard
            case 2:
                OnChangeGridSize?.Invoke(30, 16);
                break;
            
            //custom difficulty so user needs to select size
            //read the two text boxes for input and if invalid, display error in console
            case 3:
                OnChangeGridSize?.Invoke(9, 9);
                break;

            //settings
            case 4:
                //
                break;

            //quit game
            case 5:
                //
                break;

            //replay
            case 6:
                //
                break;
        }
    }
}
