using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    this class mainly handles the interactive components of the ui and makes varies tasks including 
    spawning the discs into the scene and showing the highlighted legal positions and flipping the discs and handling the user input

    it can be thought as the front-end
*/
public class GameManager : MonoBehaviour
{

/*
    the layer mask is used to check if the board is hit when using RayCast
*/
[SerializeField]
LayerMask boardLayer;

[SerializeField]
Camera cam;

[SerializeField]
Disc whiteDisc;

[SerializeField]
Disc blackDisc;

[SerializeField]
UiHandler uiHandler;

[SerializeField]
GameObject highlight;

int mode = 0;

bool mayMove = true;

List<GameObject> highlights = new List<GameObject>();

GameLogic gameLogic = new GameLogic();

Ai ai;
Ai aiWhite;

bool ready = false;


/*  
    Array of the disc gameobjects placed in the scene
*/
Disc[,] discs = new Disc[8,8];

/*
    A Player-Disc Dictionary (For Convenience)
*/
Dictionary<PlayerColor,Disc> discTurn = new Dictionary<PlayerColor, Disc>();

/*
            PLAYERPREFS

    mode = 0 PLAYER V PLAYER
    mode = 1 PLAYER V AI
    mode = 2 AI V AI
*/


private void Start() 
{
    mode = PlayerPrefs.GetInt("mode");

    if (mode==1)
    {
        ai = new Ai(PlayerPrefs.GetInt("blackai"),PlayerColor.BLACK);
    }
    else if(mode==2)
    {
        ai = new Ai(PlayerPrefs.GetInt("sliderBlack"),PlayerColor.BLACK);
        aiWhite = new Ai(PlayerPrefs.GetInt("sliderWhite"),PlayerColor.WHITE);
    }
    ready=true;

    
    /*  
        initiate the player-disc Dicitonary
    */
    initialSetup();
    /*  
        Place the starting discs
    */
    placeStartingDiscs();

    /*
        Show Legal Plays
    */
    showLegalPlays();

}

/*
    this method shows the highlighted positions of the legal moves 
*/
private void showLegalPlays()
{
    foreach(Position legalPosition in gameLogic.legalMoves.Keys)
    {
        Vector3 scenePosition = positionBoardToScene(legalPosition) + Vector3.up*0.1f;
        highlights.Add(Instantiate(highlight, scenePosition, Quaternion.identity));
    }
}

/*
    this method hides the highlighted positions of the legal moves 
*/
private void hideLegalPlays()
{
    highlights.ForEach(Destroy);
    highlights.Clear();
}

/*
    Initializing the player-disc dictionary
*/
private void initialSetup()
{
    discTurn[PlayerColor.BLACK] = blackDisc;
    discTurn[PlayerColor.WHITE] = whiteDisc;
}

/*
    handling user input (clicking on the scene)
*/
private void handleBoardPositionHit()
{
    Position boardPosition;
    Ray ray = cam.ScreenPointToRay(Input.mousePosition); 
    if(Physics.Raycast(ray,out RaycastHit hitData, Mathf.Infinity, boardLayer))
    {
        boardPosition = positionSceneToBoard(hitData.point);
        /*
            Check if player can place here , else return
        */

        if(gameLogic.canTakeMove(boardPosition,out Move move))
        {
            StartCoroutine(makeMove(move));
        }
    }
}


private void Update() 
{   
    if(mayMove && !gameLogic.gameOver && ready)
    {
        if(mode==0 || (mode==1 && gameLogic.currentPlayer==PlayerColor.WHITE ))
        {
            if(Input.GetMouseButtonDown(0))
            {
                handleBoardPositionHit();
            }
        }
        else if(mode==1 && gameLogic.currentPlayer==PlayerColor.BLACK && !gameLogic.gameOver)
        {
            Move bestMove = ai.getBestMove(gameLogic.currentState);

            if(gameLogic.canTakeMove(bestMove.destination,out Move move))
            {
                StartCoroutine(makeMove(move));
            }
        }

        else if(mode==2)
        {
            if(gameLogic.currentPlayer==PlayerColor.BLACK)
            {
                Move bestMove = ai.getBestMove(gameLogic.currentState);
                if(gameLogic.canTakeMove(bestMove.destination,out Move move))
                {
                    StartCoroutine(makeMove(move));
                }
            }
            else
            {
                Move bestMove = aiWhite.getBestMove(gameLogic.currentState);
                if(gameLogic.canTakeMove(bestMove.destination,out Move move))
                {
                    StartCoroutine(makeMove(move));
                }
            }
        }
    }
    
}


/*
    Changes the scene coordinates to the position on the board
    (x,y,z) -> (row,column)
*/
    Position positionSceneToBoard(Vector3 scenePosition)
    {
        int column = (int)(scenePosition.x - 0.25); //floor(x-0.25)
        int row =  7 - (int)(scenePosition.z - 0.25); // 7 - floor(z-0.25);
        return new Position(row,column);
    } 


/*
    Changes the position on the board to the scene coordinates
    (row,column) -> (x,y,z)
*/
    Vector3 positionBoardToScene(Position boardPosition)
    {
        float x = 0.75f+boardPosition.column;
        float z = 7.75f-boardPosition.row;
        return new Vector3(x,0,z);
    }

/*
    spawns the discs into scene and keep track of them inside the discs array
*/
    private void placeDisc(Disc disc , Position boardPosition)
    {
        /*
            Vector3.up*0.1f is added to show the whole disc when placed
        */
        Vector3 scenePosition = positionBoardToScene(boardPosition) + Vector3.up*0.1f;
        discs[boardPosition.row,boardPosition.column] = Instantiate(disc, scenePosition, Quaternion.identity);
    } 

/*
    called at the start , it spawns the initial four discs
*/
    private void placeStartingDiscs()
    {
        foreach(Position pos in gameLogic.getOccupiedPositions())
        {
            placeDisc(discTurn[gameLogic.currentState[pos.row,pos.column]],pos);
        }
    }

/*
    utlity function to flip array of discs
*/
    private void flipDiscs(List<Position>outFlanked)
    {
        foreach(Position pos in outFlanked)
        {
            discs[pos.row,pos.column].flipDisc();
        }
    }

/*
    this is the real show,
    this function starts by hiding the old legal moves then spawn the new disc
    and then flips the outflanked ones and then shows the new legal moves
    it also uses some ui functions to update the score and to change the playerTurn text
    and surely at the end checks if the game is over

    NOTE:
        it was made a coroutine because we needed to use the waitForSeconds() to wait for the animation to be done
*/
    private IEnumerator makeMove(Move move)
    {
        mayMove=false;
        hideLegalPlays();
        placeDisc(discTurn[move.player],move.destination);
        yield return new WaitForSeconds(0.250f); //length of the place disc animation
        flipDiscs(move.outFlanked);
        yield return new WaitForSeconds(0.833f);
        showLegalPlays();
        uiHandler.showScore(gameLogic.score[PlayerColor.WHITE],gameLogic.score[PlayerColor.BLACK]);
        uiHandler.ChangePlayerTurn(gameLogic.currentPlayer);
        checkGameOver();
        mayMove=true;
    }


    private void checkGameOver()
    {
        if(gameLogic.gameOver)
        {
            uiHandler.showGameOver(gameLogic.winner,gameLogic.score[PlayerColor.WHITE],gameLogic.score[PlayerColor.BLACK]);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
