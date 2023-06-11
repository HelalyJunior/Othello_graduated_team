using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    class representing the Disc game object (ui component)
*/
public class Disc : MonoBehaviour
{
    [SerializeField]
    PlayerColor colorUp;

    Animator animator;

    private void Start() 
    {
        animator = GetComponent<Animator>();
    }

    public void flipDisc()
    {
        if(colorUp==PlayerColor.BLACK)
        {
            animator.Play("blackToWhite");
            colorUp=PlayerColor.WHITE;
        }
        else
        {
            animator.Play("whiteToBlack");
            colorUp=PlayerColor.BLACK;
        }
    }



}
