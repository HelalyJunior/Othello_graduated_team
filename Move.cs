using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    class representing a move 
*/
public class Move
{
    /*
        the player taking the move
    */
    public PlayerColor player;

    /*
        the outflanked positons
    */
    public List<Position> outFlanked;

    /*
        the positons the player moving to
    */
    public Position destination;


    public Move()
    {
        
    }

    public Move(PlayerColor player,List<Position> outFlanked , Position destination)
    {
        this.player = player;
        this.outFlanked = outFlanked;
        this.destination = destination;
    }

}
