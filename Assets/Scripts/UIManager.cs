using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public delegate void ChangeGridSize(int newLength, int newWidth);
    public static event ChangeGridSize OnChangeGridSize;

    public TMP_InputField userInput;

    // Start is called before the first frame update
    public void ButtonClicked(int id)
    {
        Debug.Log($"You clicked a menu button with id: {id}");
        
        switch(id)
        {
            case 0:
                OnChangeGridSize?.Invoke(5, 5);
                //OnChangeSize?.Invoke(9, 9);
                break;

            case 1:
                OnChangeGridSize?.Invoke(16, 16);
                break;

            case 2:
                OnChangeGridSize?.Invoke(30, 16);
                break;
            
            //custom difficulty so user needs to select size
            case 3:
                OnChangeGridSize?.Invoke(9, 9);
                break;

        }
    }
}
