/*
    enum representing the different states of a tile 
*/
public enum PlayerColor
{
    NULL,BLACK,WHITE
}

public class Player
{
    /*
        a utility function that returns the opponent
    */
    public static PlayerColor getOpponent(PlayerColor playerColor)
    {
        if(playerColor==PlayerColor.WHITE)
        {
            return PlayerColor.BLACK;
        }
        else
        {
            return PlayerColor.WHITE;
        }
    }
}
