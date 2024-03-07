using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class GameMechanics : MonoBehaviour
{
    public GameObject cellPrefab;

    public GameObject mainMenu;

    public int difficulty;

    public int numBombs;

    public int length;

    public int width;

    public bool gameOver = false;

    RaycastHit tmpHitHighlight;

    public GameObject[,] boardData;
    
    //listener to update grid size
    private void OnEnable()
    {
        UIManager.OnChangeGridSize += UIManager_OnChangeGridSize;
    }
    
    //event to change grid size
    private void UIManager_OnChangeGridSize(int newLength, int newWidth)
    {
        CreateBoard(newLength, newWidth);
        
        //hide menu
        mainMenu.transform.gameObject.SetActive(false);
        
        //reset state of game
        gameOver = false;
    }
    
    //listener to disable grid size change
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

        //set boardData to size of the area
        boardData = new GameObject[length, width];

        //create the locations for the bombs to be placed
        int[] bombLocs = assignBombLocs(area);

        //makes cells for the board
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //create cellPrefab and assign transform information
                var go = GameObject.Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);
                go.transform.position = new Vector3(i, j, 0);
                go.transform.Rotate(270, 0, 0);
                go.transform.name = $"[{i}, {j}]";
                go.tag = "Cell";

                //insert the newly made game object into boardData
                boardData[i,j] = go;

                //assign row and column in component
                var cd = go.transform.GetComponent<CellLogic>();
                cd.row = i;
                cd.col = j;

                //assign (isBomb / value) if the curr cell (is / is not) a bomb
                if ((Array.IndexOf(bombLocs, ((i * length) + j))) != -1)
                {
                    //cell is a bomb, so assign isBomb to true
                    cd.IsBomb = true;
                    cd.cellValue = -1;
                } else
                {
                    //since the cell is not a bomb, we assign it the value of 0
                    cd.cellValue = 0;
                }
            }
        }
        //update the values of the non-bomb elements
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

        //perform ray casting only if not game over
        if ((Input.GetMouseButtonDown(0)) && !gameOver)
        {
            //left click

            //tmpHitHighlight - captures what is in the path from the ray
            //can also grab other info such as face, rotation, etc. from the `tmpHitHighlight` object 

            //100 represents the meters (distance) from the src (camera) outwards
            if (Physics.Raycast(ray, out tmpHitHighlight, 100))
            {
                var cd = tmpHitHighlight.transform.GetComponentInChildren<CellLogic>();
                //Debug.Log($"Hit: {cd}");

                //make text visible
                cd.tmpCellValue.transform.gameObject.SetActive(true);

                //hide the button part of the cell
                cd.pressedButton.gameObject.SetActive(false);

                cd.selected = true;

                //make bomb visible if there is a bomb
                if (cd.IsBomb)
                {
                    cd.bombRef.transform.gameObject.SetActive(true);

                    //player has lost the game
                    loseGame();
                }
                else
                {
                    //change text to value to be displayed
                    cd.tmpCellValue.text = $"{cd.cellValue}";

                    //if the current value of the cell is 0, reveal all numerical cells
                    if (cd.cellValue == 0)
                    {
                        revealZeroNeighbors(cd);
                    }
                }

                //make flag invisible
                cd.flag = false;
                cd.flagParent.transform.gameObject.SetActive(false);
            }
        }
        else if ((Input.GetMouseButtonDown(1)) && !gameOver)
        {
            //right click

            //check what is in the path
            if (Physics.Raycast(ray, out tmpHitHighlight, 100))
            {
                var cd = tmpHitHighlight.transform.GetComponentInChildren<CellLogic>();

                //only allow/disallow flag if the current cell is not selected === number showing === clicked
                if (!(cd.selected))
                {
                    //flip flag visibility
                    cd.flag = !(cd.flag);
                    cd.flagParent.transform.gameObject.SetActive(cd.flag);
                }
            }
        }
    }

    //given a cell with a value of 0, reveal all neighbors
    private void revealZeroNeighbors(CellLogic zeroCell)
    {
        //obtain the information from zeroCell
        int currRow = zeroCell.row;
        int currCol = zeroCell.col;

        //find all neighbors
        for (int k = currRow - 1; k < currRow + 2; k++)
        {
            for (int l = currCol - 1; l < currCol + 2; l++)
            {
                //check if k and l are within bounds of board
                //also check that [k,l] =/= [i,j]
                if (k >= 0 && k < length &&
                    l >= 0 && l < width &&
                    !(k == currRow && l == currCol))
                {
                    //obtain neighbor since the position is valid
                    var neighbor = boardData[k, l].GetComponentInChildren<CellLogic>();

                    //if the neighbor is not selected 
                    if (!(neighbor.selected))
                    {
                        //change text to the neighbor's value and make text visible
                        neighbor.tmpCellValue.text = $"{neighbor.cellValue}";
                        neighbor.tmpCellValue.transform.gameObject.SetActive(true);

                        //hide the button part of the cell
                        neighbor.pressedButton.gameObject.SetActive(false);

                        //make cell selected
                        neighbor.selected = true;

                        //make flag invisible
                        neighbor.flag = false;
                        neighbor.flagParent.transform.gameObject.SetActive(false);

                        //if the neighbor has a value of 0, then recurse
                        if (neighbor.cellValue == 0)
                        {
                            //get all connected zero's to this neighbor
                            revealZeroNeighbors(neighbor);
                        }
                    }

                    
                }
            }
        }
    }

    

    //obtains locations for bombs given an area(board dims)
    private int[] assignBombLocs(int area)
    {
        //decide how many bombs to place based on area
        //the number of bombs to place is roughly the square root of the area of the board
        numBombs = Mathf.RoundToInt(Mathf.Sqrt(area));

        //create container for the bomb locations
        int[] bombLocs = new int[numBombs];

        //plant the bombs in random positions on the board
        for (int i = 0; i < numBombs; i++)
        {
            bombLocs[i] = UnityEngine.Random.Range(0, area);
        }

        return bombLocs;
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
        //i.e. the logic still flows like normal

        //begin coroutine to handle losing the game
        StartCoroutine(lostGameTimout());

        Debug.Log($"You Lost :(");

        //disallow button clicks
        gameOver = true;
    }

    //after 4 seconds handles lost game status
    private IEnumerator lostGameTimout()
    {
        Debug.Log($"Timeout for {4} seconds");
        yield return new WaitForSeconds(4f);

        //bring up the main menu where player can select options again
        mainMenu.transform.gameObject.SetActive(true);

        
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