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

    public GameLogic()
    {
    }
}
