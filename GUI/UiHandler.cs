using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
    this class handles the static ui members such as texts and sprites
*/
public class UiHandler : MonoBehaviour
{
    [SerializeField]
    TMP_Text playerTurn;

    [SerializeField]
    TMP_Text whiteScore;

    [SerializeField]
    TMP_Text blackScore;


    [SerializeField]
    TMP_Text whiteScoreWinner;

    [SerializeField]
    TMP_Text blackScoreWinner;

    [SerializeField]
    Image overlay; 

    [SerializeField]
    TMP_Text winner; 


    /*  
        updates the player's turn text
    */
    public void ChangePlayerTurn(PlayerColor currentTurn)
    {
        if(currentTurn==PlayerColor.WHITE)
        {
            playerTurn.text="<sprite name=WhiteUp>White's Turn";
        }
        else
        {
            playerTurn.text="<sprite name=BlackUp>Black's Turn";
        }
    }

    /*  
        updates the score text
    */
    public void showScore(int white , int black)
    {
        whiteScore.text="<sprite name=WhiteUp>"+white;
        blackScore.text="<sprite name=BlackUp>"+black;
    }

    /*
        shows the gameover panel 
    */
    public void showGameOver(PlayerColor player,int white , int black)
    {
        whiteScore.enabled=false;
        blackScore.enabled=false;
        playerTurn.enabled=false;
        overlay.gameObject.SetActive(true);
        winner.gameObject.SetActive(true);
        whiteScoreWinner.text="<sprite name=WhiteUp>"+white;
        blackScoreWinner.text="<sprite name=BlackUp>"+black;
        if(player==PlayerColor.BLACK)
        {
            winner.text="Black Player Wins";
        }
        else if(player==PlayerColor.WHITE)
        {
            winner.text="White Player Wins";
        }
        else
        {
            winner.text="Draw , this is just sad";
        }
    }
}
