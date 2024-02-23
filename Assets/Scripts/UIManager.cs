using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public delegate void ChangeSize(int newLength, int newWidth);
    public static event ChangeSize OnChangeSize;

    // Start is called before the first frame update
    public void ButtonClicked(Button id)
    {
        Debug.Log($"You clicked a button with id: {id.transform.name}");
        
        
        switch(id.transform.name)
        {
            case "EasyButton":
                OnChangeSize?.Invoke(5, 5);
                //OnChangeSize?.Invoke(9, 9);
                break;

            case "MediumButton":
                OnChangeSize?.Invoke(16, 16);
                break;

            case "HardButton":
                OnChangeSize?.Invoke(30, 16);
                break;
            
            //custom difficulty so user needs to select size
            case "CustomDifficultyButton":
                OnChangeSize?.Invoke(9, 9);
                break;

            
        }
        

    }
}
