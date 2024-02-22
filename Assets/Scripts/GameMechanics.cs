using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameMechanics : MonoBehaviour
{
    public GameObject cellPrefab;

    public int difficulty = 0;

    public int numBombs = 10;

    public int length = 9;
    
    public int width = 9;

    RaycastHit tmpHitHighlight;

    public int[,] boardData;

    /*
    //listener to update grid size
    private void onEnable()
    {
        UIManager.OnChangeGridSize += UIManager_OnChangeGridSize;
    }

    //changes grid size
    private void UIManager.OnChangeGridSize(int m, int n)
    {
        CreateBoard(m, n);
    }

    //disable grid size change
    private void OnDisable()
    {
        UIManager.OnChangeGridSize() -= UIManager_OnChangeGridSize;
    }
    */

    void CreateBoard(int newLength, int newWidth)
    {
        length = newLength;
        width = newWidth;

        //set boardData to 0's
        boardData = new int[length, width];

        int numBombsSet = 0;

        //makes tiles
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //create cellPrefab
                var go = GameObject.Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);
                go.transform.position = new Vector3(i, j, 0);
                go.transform.Rotate(270, 0, 0);

                //go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.name = $"[{i}, {j}]";

                var cd = go.transform.GetComponent<CellLogic>();
                cd.row = i;
                cd.col = j;

                //can we make more bombs
                if (numBombsSet < numBombs)
                {
                    //determine if the current tile should be a bomb or not
                    //adjust randomization into this
                    if (Random.Range(1,10) > 7)
                    {
                        cd.IsBomb = true;
                        numBombsSet++;

                        //update boardData to bomb
                        boardData[i, j] = -1;
                    }
                }
            }
        }

        updateBoardData();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        //difficulty selected determines some of the other game vars such as length
        switch(difficulty)
        {
            //easy
            case 0:
                numBombs = 10;
                length = width = 9;
                break;

            //medium
            case 1:
                numBombs = 32;
                length = width = 16;
                break;

            //hard
            case 2:
                numBombs = 60;
                length =  30;
                width = 16;
                break;

            //customizable size
            case 3:
                //numBombs \exists in range of 10% to 20% of the total tiles being bombs
                //dims must be >= 2
                numBombs = -1;
                length = 10;
                width = 10;
                break;

            //improperly defined
            default:
                numBombs = 0;
                length = width = 1;
                Debug.Log($"Improperly defined difficulty. Please Select from the following: 0, 1, 2, 3");
                Debug.Log($"0 = Easy\n1 = Medium\n2 = Large\n3 = Custom Size");
                break;
        }

        */

        //sets up the tiles to be used in game
        //number of current bombs set



        //other game setup options

        Debug.Log($"Game Setup Complete!");

        CreateBoard(9,9);
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

                //hide the button
                cd.pressedButton.gameObject.SetActive(false);

                //make bomb visible if there is a bomb
                if(cd.IsBomb)
                {
                    cd.bombRef.transform.gameObject.SetActive(true);
                    //change text to value to be displayed
                    //cd.tmpCellValue.text = $" ";
                } else
                {
                    //change text to value to be displayed
                    cd.tmpCellValue.text = $"{boardData[cd.row, cd.col]}";
                }
                
            }
        }
    }

    private void FixedUpdate()
    {
        //always updates via clock speed
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
                //if current elt in board is not a bomb, find it's value
                if (boardData[i, j] != -1)
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
                                !(k == i && l == j) &&
                                boardData[k, l] == -1)
                            {
                                //increment value
                                boardData[i, j] += 1;
                            }
                        }
                    }
                }
            }
        }
    }
}