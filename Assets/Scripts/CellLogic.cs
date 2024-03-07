using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CellLogic : MonoBehaviour
{
    //current cell is a bomb or not
    public bool IsBomb;

    //current cell has been left clicked
    public bool selected = false;

    //value for the current cell
    public int cellValue;

    //has the current cell been flagged?
    public bool flag;

    //references to the transforms we will be using in the prefab
    public Transform bombRef;
    public Transform pressedButton;
    public Transform cellBase;
    public Transform flagParent;

    //name of the created cell
    public string CellId => $"{row}, {col}";

    //data used for name and indexing
    public int row;
    public int col;

    //what text to display
    public TMP_Text tmpCellValue;

    // Start is called before the first frame update
    void Start()
    {
        bombRef.gameObject.SetActive(false);
        tmpCellValue.transform.gameObject.SetActive(false);
        tmpCellValue.text = " ";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
