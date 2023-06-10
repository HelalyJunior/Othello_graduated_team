/*
    this class represents a position on a board
*/

public class Position
{
    /*
        rows are counted top to bottom 0 -> 7
        columns are counted left to right 0 -> 7 
    */
    public int row;
    public int column;

    public Position(int row , int column)
    {
        this.row = row;
        this.column = column;
    }

    public override bool Equals(object obj)
    {
        if(obj is Position pos)
        {
            return this.row==pos.row && this.column==pos.column;
        }
        return false;
    }

    /*
        this has to be overriden because we are using positions inside a dictionary
    */
    public override int GetHashCode()
    {
        return row*8 + column;
    }
}
