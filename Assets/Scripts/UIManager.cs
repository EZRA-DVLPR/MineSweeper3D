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

    public delegate void ChangePanel(int newPanel);
    public static event ChangePanel OnChangePanel;

    public GameObject LengthBox;

    public GameObject WidthBox;

    public GameObject VolumeSlider;

    [SerializeField] public AudioSource buttonClickSoundEffect;

    [SerializeField] public AudioSource TogglePressSoundEffect;

    //plays sound when toggle is made
    public void ToggleButton(bool toggle)
    {
        TogglePressSoundEffect.Play();

        //handle toggle information
    }

    //handle events for buttons being clicked on menus
    public void ButtonClicked(int id)
    {
        buttonClickSoundEffect.Play();

        switch (id)
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
                //get the info from the text boxes
                string givenLength = LengthBox.transform.Find("Text Area/Text").GetComponent<TMP_Text>().text.Replace("\u200B", "");
                string givenWidth = WidthBox.transform.Find("Text Area/Text").GetComponent<TMP_Text>().text.Replace("\u200B", "");

                //make int versions of length and width
                int givenLengthAsInt = 0;
                int givenWidthAsInt = 0;

                //set arbitrarily high number of cells for dim max (1000)
                if ((Int32.TryParse(givenLength, out givenLengthAsInt)) && (Int32.TryParse(givenWidth, out givenWidthAsInt)))
                {
                    if ((givenLengthAsInt > 1) && (givenLengthAsInt <= 1000) && 
                       (givenWidthAsInt > 1) && (givenWidthAsInt <= 1000))
                    {
                        OnChangeGridSize?.Invoke(givenLengthAsInt, givenWidthAsInt);
                    } else
                    {
                        Debug.Log($"Invalid integer bounds for length/width. Please enter a value between 1 and 1001");
                    }
                }
                else
                {
                    Debug.Log($"failed to parse length/width input as int");
                }
                break;

            //settings
            case 4:
                OnChangePanel?.Invoke(4);
                break;

            //new game === main menu
            case 6:
                OnChangePanel?.Invoke(6);
                break;

            //back button
            case 8:
                //send back to a diff screen based on previous panel
                switch(GameMechanics.prevPanel)
                {
                    //back to main menu
                    case 6:
                        OnChangePanel?.Invoke(6);
                        break;

                    //back to post-game menu
                    case 7:
                        OnChangePanel?.Invoke(7);
                        break;
                }
                break;

            case 9:
                exitGame();
                break;
        }
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
