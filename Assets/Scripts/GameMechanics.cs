using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using TMPro;

public class GameMechanics : MonoBehaviour
{
    public Stopwatch stopwatch;

    public GameObject cellPrefab;

    public GameObject Canvas;

    public static int prevPanel;

    public static int currPanel = 6;

    public int difficulty;

    public int numBombs;

    public int length;

    public int width;

    public static bool gameOver = false;

    public static bool playingGame = false;

    public GameObject[,] boardData;

    RaycastHit tmpHitHighlight;
        
    //event to change grid size
    private void UIManager_OnChangeGridSize(int newLength, int newWidth)
    {
        //create a new board for the game with given dims
        CreateBoard(newLength, newWidth);
        
        //hide main menu
        Canvas.transform.Find("Main Menu Panel").transform.gameObject.SetActive(false);
        
        //reset state of game
        gameOver = false;

        //start timer for game
        stopwatch.startTimer();
    }

    //changes panels (menus)
    private void UIManager_OnChangePanel(int newPanel)
    {
        //hide the current panel and if not settings => prevPanel = currPanel
        switch (currPanel)
        {
            //Settings
            case 4:
                Canvas.transform.Find("Settings Panel").transform.gameObject.SetActive(false);
                break;

            //Main menu
            case 6:
                Canvas.transform.Find("Main Menu Panel").transform.gameObject.SetActive(false);
                prevPanel = 6;
                break;

            //Post game
            case 7:
                Canvas.transform.Find("Post-Game Panel").transform.gameObject.SetActive(false);
                prevPanel = 7;
                break;
        }

        //show only the new panel and make it the current panel
        switch (newPanel)
        {
            //Settings
            case 4:
                Canvas.transform.Find("Settings Panel").transform.gameObject.SetActive(true);
                currPanel = 4;
                break;

            //Main Menu
            case 6:
                Canvas.transform.Find("Main Menu Panel").transform.gameObject.SetActive(true);
                currPanel = 6;
                break;

            //Post Game
            case 7:
                Canvas.transform.Find("Post-Game Panel").transform.gameObject.SetActive(true);
                currPanel = 7;
                break;
        }
    }

    //stop all listeners when game ends
    private void OnDisable()
    {
        UIManager.OnChangeGridSize -= UIManager_OnChangeGridSize;
        UIManager.OnChangePanel -= UIManager_OnChangePanel;
        stopwatch.OnStartTimer -= Stopwatch_HandleTimerStart;
        stopwatch.OnStopTimer -= Stopwatch_HandleTimerStop;
    }

    private void Stopwatch_HandleTimerStart()
    {
        Debug.Log($"Stopwatch Started");
    }

    private void Stopwatch_HandleTimerStop()
    {
        Debug.Log($"Stopwatch Stopped");
    }

    public void CreateBoard(int newLength, int newWidth)
    {
        //reset panel info
        currPanel = 6;
        prevPanel = 0;

        //update stored length and width
        length = newLength;
        width = newWidth;

        //create area to make calculations easier later on
        int area = length * width;

        //set boardData to size of the area
        boardData = new GameObject[length, width];

        //create the locations for the bombs to be placed
        int[,] bombLocs = assignBombLocs();

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
                go.transform.gameObject.SetActive(true);

                //insert the newly made game object into boardData
                boardData[i,j] = go;

                //assign row and column in component
                var cd = go.transform.GetComponent<CellLogic>();
                cd.row = i;
                cd.col = j;

                //assign (isBomb / value) if the curr cell (is / is not) a bomb
                if (bombLocs[i,j] == -1)
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

        //set playing Game status => allows pausing
        playingGame = true;

        //reset panel info
        currPanel = 0;
        prevPanel = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        //start listener for changing grid size
        UIManager.OnChangeGridSize += UIManager_OnChangeGridSize;

        //start listener for updating panel
        UIManager.OnChangePanel += UIManager_OnChangePanel;

        //start listeners for start and stop timer
        stopwatch.OnStartTimer += Stopwatch_HandleTimerStart;
        stopwatch.OnStopTimer += Stopwatch_HandleTimerStop;

        Debug.Log($"Game Setup Complete!");
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
                    //current cell is not a bomb

                    //change text to value to be displayed
                    cd.tmpCellValue.text = $"{cd.cellValue}";

                    //if the current value of the cell is 0, reveal all numerical cells
                    if (cd.cellValue == 0)
                    {
                        revealZeroNeighbors(cd);
                    }

                    //assume the game has been completed
                    bool gameComplete = true;

                    //if the player has selected all of the numerical cells, they win
                    foreach (GameObject go in boardData)
                    {
                        var elt = go.transform.GetComponentInChildren<CellLogic>();
                        if ((elt.cellValue >= 0) && (!(elt.selected))) 
                        {
                            gameComplete = false;
                            break;
                        }
                    }

                    //handle win
                    if (gameComplete)
                    {
                        winGame();
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

                //only show/hide flag if the current cell is not selected === number showing === clicked
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
    private int[,] assignBombLocs()
    {
        //decide how many bombs to place based on area => randomly selecte a percentage from 12 to 28 of the area
        int numBombs = Mathf.RoundToInt(length * width * UnityEngine.Random.Range(0.12f, 0.28f));

        //create container for the bomb locations
        int[,] bombLocs = new int[length, width];

        int plantedBombs = 0;

        //plant the bombs in random positions on the board
        while (plantedBombs < numBombs)
        {
            int col = UnityEngine.Random.Range(0, length);
            int row = UnityEngine.Random.Range(0, width);

            //plant a bomb in a new bomb position
            if (bombLocs[col, row] != -1)
            {
                bombLocs[col, row] = -1;
                plantedBombs++;
            }
        }

        return bombLocs;
    }

    //handles vars when player wins game
    private void winGame()
    {
        //put flags on all the bombs if they aren't there already
        foreach (GameObject go in boardData)
        {
            var elt = go.transform.GetComponentInChildren<CellLogic>();
            if (elt.cellValue < 0)
            {
                //make flag visible
                elt.flag = true;
                elt.flagParent.transform.gameObject.SetActive(true);
            }
        }

        StartCoroutine(endGameTimout());

        Debug.Log($"You Win! :)");
    }

    //handles vars when player loses game
    private void loseGame()
    {
        //begin coroutine to handle losing the game
        StartCoroutine(endGameTimout());

        Debug.Log($"You Lost :(");
    }

    //after 4 seconds handles lost game status
    private IEnumerator endGameTimout()
    {
        //disallow pausing
        playingGame = false;

        //destroy all cells in board after 4 seconds
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Cell");
        foreach (GameObject gObj in objectsWithTag)
        {
            Destroy(gObj, 4f);
        }

        Debug.Log($"Timeout for {4} seconds");

        //write to the textbox what the time was
        Canvas.transform.Find("Post-Game Panel/TimeTakenActual").GetComponentInChildren<TMP_Text>().text = stopwatch.stopTimer();

        //disallow button clicks
        gameOver = true;

        yield return new WaitForSeconds(4f);

        //bring up the main menu where player can select options again
        Canvas.transform.Find("Post-Game Panel").transform.gameObject.SetActive(true);

        //reset panel info
        currPanel = 7;
        prevPanel = 0;
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