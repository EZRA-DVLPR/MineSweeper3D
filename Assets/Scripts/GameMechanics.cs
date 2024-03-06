using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class GameMechanics : MonoBehaviour
{
    public GameObject cellPrefab;

    public int difficulty;

    public int numBombs;

    public int length = 9;
    
    public int width = 9;

    RaycastHit tmpHitHighlight;

    public GameObject[,] boardData;

    
    //listener to update grid size
    private void OnEnable()
    {
        UIManager.OnChangeGridSize += UIManager_OnChangeGridSize;
    }
    
    //changes grid size
    private void UIManager_OnChangeGridSize(int newLength, int newWidth)
    {
        CreateBoard(newLength, newWidth);
        GameObject tempObject = GameObject.FindGameObjectWithTag("Main Menu");
        //hide it
        tempObject.transform.gameObject.SetActive(false);
    
    }
    
    //disable grid size change
    private void OnDisable()
    {
        UIManager.OnChangeGridSize -= UIManager_OnChangeGridSize;
    }
    

    public void CreateBoard(int newLength, int newWidth)
    {
        //update stored length and width
        length = newLength;
        width = newWidth;

        //create area to make calculations easier later on
        int area = length * width;

        //set boardData to size of the length * width
        boardData = new GameObject[length, width];

        //decide how many bombs to place
        //the number of bombs to place is roughly the square root of the area of the board
        numBombs = Mathf.RoundToInt(Mathf.Sqrt(area));

        //create the locations for the bombs to be placed
        int[] bombLocs = new int[numBombs];

        //plant the bombs in random positions on the board
        for (int i = 0; i < numBombs; i++)
        {
            bombLocs[i] = UnityEngine.Random.Range(0, area);
        }

        //makes cells for board
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //create cellPrefab and assign transform information
                var go = GameObject.Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);
                go.transform.position = new Vector3(i, j, 0);
                go.transform.Rotate(270, 0, 0);
                go.transform.name = $"[{i}, {j}]";

                //insert the newly made game object into the boardData
                boardData[i,j] = go;

                //assign row and column in component
                var cd = go.transform.GetComponent<CellLogic>();
                cd.row = i;
                cd.col = j;

                //assign bomb if the the given index is within the bombLocs array
                if ((Array.IndexOf(bombLocs, ((i * length) + j))) != -1)
                {
                    cd.IsBomb = true;
                } else
                {
                    //since the element is not a bomb, we assign it a value of 0 
                    cd.cellValue = 0;
                }
            }
        }
        updateBoardData();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log($"Game Setup Complete!");
    }

    // Update is called once per frame
    void Update()
    {
        //2D coord -> 3D coord as a ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //perform ray casting
        if (Input.GetMouseButtonDown(0))
        {
            //tmpHitHighlight - captures what is in the path from the ray
            //can also grab other info such as face, rotation, etc. from the `tmpHitHighlight` object 

            //100 represents the meters (distance) from the src (camera) outwards
            if (Physics.Raycast(ray, out tmpHitHighlight, 100))
            {
                var cd = tmpHitHighlight.transform.GetComponentInChildren<CellLogic>();
                Debug.Log($"Hit: {cd}");

                //make text visible
                cd.tmpCellValue.transform.gameObject.SetActive(true);

                //hide the button part of the cell
                cd.pressedButton.gameObject.SetActive(false);

                //make bomb visible if there is a bomb
                if(cd.IsBomb)
                {
                    cd.bombRef.transform.gameObject.SetActive(true);
                    //change text to value to be displayed
                    //cd.tmpCellValue.text = $" ";

                    loseGame();
                } else
                {
                    //change text to value to be displayed
                    cd.tmpCellValue.text = $"{cd.cellValue}";
                }
            }
        }
    }

    //always updates via clock speed
    private void FixedUpdate()
    {
    }

    //handles vars when player loses game
    private void loseGame()
    {
        //destroy all cells in board after 4 seconds
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Cell");
        foreach (GameObject gObj in objectsWithTag)
        {
            Destroy(gObj, 4f);
        }

        //Note: there is no delay between the destruction of the cells and the debug statement below
        // i.e. the logic still flows like normal

        Debug.Log($"You Lost :(");

        //bring up the main menu where player can select options again
    }

    //updates the values within BoardData after the bombs have been set
    private void updateBoardData()
    {
        //loop to get the number for each non-bomb tile
        //search neighbors in all 8 neighboring squares
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //obtain the current element in the board
                var currCell = boardData[i, j].GetComponentInChildren<CellLogic>();

                //if current elt in board is not a bomb, find it's value
                if (!(currCell.IsBomb))
                {
                    //find neighbors that are bombs
                    for (int k = i - 1; k < i + 2; k++)
                    {
                        for (int l = j - 1; l < j + 2; l++)
                        {
                            
                            //check if k and l are within bounds of board
                            //also check that [k,l] =/= [i,j]
                            //also check if the current neighbor is a bomb
                            if (k >= 0 && k < length &&
                                l >= 0 && l < width &&
                                !(k == i && l == j))
                            {
                                //obtain neighbor since the position is valid
                                var neighbor = boardData[k, l].GetComponentInChildren<CellLogic>();

                                //if the neighbor is a bomb, then increment current cell
                                if (neighbor.IsBomb)
                                {
                                    currCell.cellValue += 1;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}