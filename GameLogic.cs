using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
    the main man , the shadow or you can call it the backend
    this class implements the rules of the game and keeps track of the 
    current player and the current gameState and current legal moves and 
    the score for each player and wether the game is over or not and if so the winner
*/
public class GameLogic
{
    /*
        this holds the currentGameState (the board)
    */
    public PlayerColor[,] currentState = new PlayerColor[8,8]; 
    /*
        holds the player on turn
    */
    public PlayerColor currentPlayer;
    public Dictionary<PlayerColor,int> score;
    
    public PlayerColor winner;
    public bool gameOver;
    /*
        holds the currentLegalMoves and is updated every turn
    */
    public Dictionary<Position,List<Position>> legalMoves; 


    /*
        this method changes the turn and checks if the game is over
        if one player doesn't have any plays , it automatically skips the turn
    */
    public void changeTurns()
    {
        currentPlayer = Player.getOpponent(currentPlayer);
        legalMoves = getLegalMoves(currentPlayer);

        if(legalMoves.Count>0)
        {
            return; //normal
        }

        currentPlayer = Player.getOpponent(currentPlayer);
        legalMoves = getLegalMoves(currentPlayer);

        if(legalMoves.Count==0)
        {
            winner=getWinner();
            gameOver=true;
        }
    }

    /*
        the name gave it away :)
    */
    public PlayerColor getWinner()
    {
        if(score[PlayerColor.WHITE]>score[PlayerColor.BLACK])
        {
            return PlayerColor.WHITE;
        }
        else if(score[PlayerColor.WHITE]<score[PlayerColor.BLACK])
        {
            return PlayerColor.BLACK;
        }
        else
        {
            return PlayerColor.NULL;
        }
    }

    /*
        checks if a certain board position is empty
    */
    private bool isPositionEmpty(Position position)
    {
        if(currentState[position.row,position.column]==PlayerColor.NULL)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
        checks if a certain board position is in bounds
    */
    private static bool isInBorder(int row , int column)
    {
        if(row>=0 && row<8 && column>=0 && column<8)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
        utility function that returns the outflanked positions in certain direction 
    */
    private List<Position> getOutFlankedByDirection(PlayerColor player , Position position , Direction direction)
    {
        int row;
        int column;

        int shiftInRows=0;
        int shiftInCols=0;

        switch(direction)
        {
            case Direction.UP:
                shiftInRows=-1;
                shiftInCols=0;
                break;
            case Direction.DOWN:
                shiftInRows=1;
                shiftInCols=0;
                break;
            case Direction.LEFT:
                shiftInRows=0;
                shiftInCols=-1;
                break;
            case Direction.RIGHT:
                shiftInRows=0;
                shiftInCols=1;
                break;
            case Direction.DIAGONALRIGHTUP:
                shiftInRows=-1;
                shiftInCols=1;
                break;
            case Direction.DIAGONALRIGHTDOWN:
                shiftInRows=1;
                shiftInCols=1;
                break;
            case Direction.DIAGONALLEFTUP:
                shiftInRows=-1;
                shiftInCols=-1;
                break;
            case Direction.DIAGONALLEFTDOWN:
                shiftInRows=1;
                shiftInCols=-1;
                break;
        }
  
        List<Position> outFlanked = new List<Position>();
        row = position.row + shiftInRows;
        column = position.column + shiftInCols;
        while(isInBorder(row,column) && currentState[row,column]!=PlayerColor.NULL)
        {
            if(currentState[row,column]==Player.getOpponent(player))
            {
                outFlanked.Add(new Position(row,column));
                row += shiftInRows;
                column += shiftInCols;
            }
            else // it means that I reached one of my discs
            {
                return outFlanked;
            }
        }
        return new List<Position>(); //empty
    }

    /*
        it returns the outflanked positions in all eight directions , 
        it basically loops over the eight directions and call getOutFlankedByDirection()
    */
    private List<Position> getOutFlanked(PlayerColor player , Position position)
    {
        List<Position> outFlanked = new List<Position>();

        Direction[] directions = new Direction[]
        {
            Direction.UP,
            Direction.DOWN,
            Direction.LEFT,
            Direction.RIGHT,
            Direction.DIAGONALRIGHTUP,
            Direction.DIAGONALRIGHTDOWN,
            Direction.DIAGONALLEFTUP,
            Direction.DIAGONALLEFTDOWN
        };
        foreach (Direction direction in directions)
        {
           outFlanked.AddRange(getOutFlankedByDirection(player,position,direction));
        }
        return outFlanked;
    }

    /*
        it returns a dictionary where each key is a legal position and the value is the list
        of the outflanked positions
    */
    private Dictionary<Position,List<Position>> getLegalMoves(PlayerColor player)
    {
        Dictionary<Position,List<Position>> legalMovesDict = new Dictionary<Position,List<Position>>();
        for(int i=0;i<8;i++)
        {
            for(int j=0; j<8; j++)
            {
                Position position = new Position(i,j);
                if(isMoveLegal(position,player,out List<Position> outFlanked))
                {
                    legalMovesDict[position] = outFlanked;
                }
            }
        }
        return legalMovesDict;
    }

    /*
        for a move to be legal , it needs to be an empty tile and needs to outflank atleast one tile
    */
    private bool isMoveLegal(Position position , PlayerColor player , out List<Position> outFlanked)
    {
        if(currentState[position.row,position.column]==PlayerColor.NULL)
        {
            outFlanked=getOutFlanked(player,position);
            return outFlanked.Count>0;
        }
        else
        {
            outFlanked=null;
            return false;
        }
    }

    /*
        a utility function used to get the occupied positons (the not NULL positions)
    */
    public List<Position> getOccupiedPositions()
    {
        List<Position> occupiedPositions = new List<Position>();
        for(int row=0; row<8;row++)
        {
            for (int col=0;col<8;col++)
                {
                    if(currentState[row,col]!=PlayerColor.NULL)
                    {
                        occupiedPositions.Add(new Position(row,col));
                    }
                }
        }
        return occupiedPositions;
    }

    /*
        a function called at the start to fill the initial state
    */
    private void fillStartingState()
    {
        currentState[3,3]=PlayerColor.WHITE;
        currentState[4,4]=PlayerColor.WHITE;
        currentState[3,4]=PlayerColor.BLACK;
        currentState[4,3]=PlayerColor.BLACK;
    }

    /*
        the scoring starts at 2 for each player
    */
    private void startScoring()
    {
        score = new Dictionary<PlayerColor, int>()
        {
            {PlayerColor.BLACK,2},
            {PlayerColor.WHITE,2}
        };
    }

    /*
        returns true if a move is ok and flase if not 
        if true the move is taken
    */
    public bool canTakeMove(Position boardPosition , out Move move)
    {
        if(legalMoves.ContainsKey(boardPosition))
        {
            move = new Move(currentPlayer,legalMoves[boardPosition],boardPosition);
            takeMove(move);
            return true;
        }
        move=null;
        return false;
    }

    /*
        the game changer
        the funciton starts with updating the scores and then changing the currentState with the new state
        and then change turns
    */
    private void takeMove(Move move)
    {

        /*
            Update The Score
        */

        updateScore(move);

        /*
            Add The disc
        */
        currentState[move.destination.row,move.destination.column]=currentPlayer;

        /*
            Flip discs
        */
        foreach( Position pos in move.outFlanked)
        {
            currentState[pos.row,pos.column]=currentPlayer;
        }

        /*
            change turns
        */
        changeTurns();

    }

    /*
        updates the score :)
    */
    private void updateScore(Move move)
    {
        score[currentPlayer]+=1;
        /*
            Update the score
        */
        foreach( Position pos in move.outFlanked)
        {
            score[currentPlayer]+=1;
            score[Player.getOpponent(currentPlayer)]-=1;
        }


    }



    public static Dictionary<Position,List<Position>> getLegalMovesGivenState(PlayerColor[,] state,PlayerColor player)
    {
        Dictionary<Position,List<Position>> legalMovesDict = new Dictionary<Position,List<Position>>();
        for(int i=0;i<8;i++)
        {
            for(int j=0; j<8; j++)
            {
                Position position = new Position(i,j);
                if(isMoveLegalGivenState(position,player,out List<Position> outFlanked,state))
                {
                    legalMovesDict[position] = outFlanked;
                }
            }
        }
        return legalMovesDict;
    }

    public static bool isMoveLegalGivenState(Position position , PlayerColor player , out List<Position> outFlanked,PlayerColor[,] currentState)
    {
        if(currentState[position.row,position.column]==PlayerColor.NULL)
        {
            outFlanked=getOutFlankedGivenState(player,position,currentState);
            return outFlanked.Count>0;
        }
        else
        {
            outFlanked=null;
            return false;
        }
    }

    public static List<Position> getOutFlankedGivenState(PlayerColor player , Position position,PlayerColor[,] currentState)
    {
        List<Position> outFlanked = new List<Position>();

        Direction[] directions = new Direction[]
        {
            Direction.UP,
            Direction.DOWN,
            Direction.LEFT,
            Direction.RIGHT,
            Direction.DIAGONALRIGHTUP,
            Direction.DIAGONALRIGHTDOWN,
            Direction.DIAGONALLEFTUP,
            Direction.DIAGONALLEFTDOWN
        };
        foreach (Direction direction in directions)
        {
           outFlanked.AddRange(getOutFlankedByDirectionGivenState(player,position,direction,currentState));
        }
        return outFlanked;
    }

    public static List<Position> getOutFlankedByDirectionGivenState(PlayerColor player , Position position , Direction direction , PlayerColor[,] state)
    {
        int row;
        int column;

        int shiftInRows=0;
        int shiftInCols=0;

        switch(direction)
        {
            case Direction.UP:
                shiftInRows=-1;
                shiftInCols=0;
                break;
            case Direction.DOWN:
                shiftInRows=1;
                shiftInCols=0;
                break;
            case Direction.LEFT:
                shiftInRows=0;
                shiftInCols=-1;
                break;
            case Direction.RIGHT:
                shiftInRows=0;
                shiftInCols=1;
                break;
            case Direction.DIAGONALRIGHTUP:
                shiftInRows=-1;
                shiftInCols=1;
                break;
            case Direction.DIAGONALRIGHTDOWN:
                shiftInRows=1;
                shiftInCols=1;
                break;
            case Direction.DIAGONALLEFTUP:
                shiftInRows=-1;
                shiftInCols=-1;
                break;
            case Direction.DIAGONALLEFTDOWN:
                shiftInRows=1;
                shiftInCols=-1;
                break;
        }
  
        List<Position> outFlanked = new List<Position>();
        row = position.row + shiftInRows;
        column = position.column + shiftInCols;
        while(isInBorder(row,column) && state[row,column]!=PlayerColor.NULL)
        {
            if(state[row,column]==Player.getOpponent(player))
            {
                outFlanked.Add(new Position(row,column));
                row += shiftInRows;
                column += shiftInCols;
            }
            else // it means that I reached one of my discs
            {
                return outFlanked;
            }
        }
        return new List<Position>(); //empty
    }

    public static PlayerColor[,] takeMoveGivenState( PlayerColor[,] currentState , Move move,PlayerColor player , out PlayerColor turn)
    {
        PlayerColor[,] state = (PlayerColor[,])currentState.Clone();
        state[move.destination.row,move.destination.column]=player;

        /*
            Flip discs
        */
        foreach( Position pos in move.outFlanked)
        {
            state[pos.row,pos.column]=player;
        }
        turn = changeTurnsGivenState(currentState,player);
        return state;
    }


    public static PlayerColor changeTurnsGivenState(PlayerColor[,] currentState , PlayerColor currentPlayer)
    {
        if(getLegalMovesGivenState(currentState,Player.getOpponent(currentPlayer)).Count>0)
        {
            return Player.getOpponent(currentPlayer); //normal
        }

        return currentPlayer;
    }

    public static bool isGameOverGivenState(PlayerColor[,] state)
    {
        if(getLegalMovesGivenState(state,PlayerColor.BLACK).Count==0 && getLegalMovesGivenState(state,PlayerColor.WHITE).Count==0)
        {
            return true;
        }
        return false;
    }
    public GameLogic()
    {
        fillStartingState();
        startScoring();
        currentPlayer = PlayerColor.BLACK;
        legalMoves = getLegalMoves(currentPlayer);
    }
}
