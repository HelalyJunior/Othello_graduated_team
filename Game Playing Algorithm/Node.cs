using System.Collections.Generic;

public class Node
{
    public PlayerColor[,] state;

    public PlayerColor turn;
    List<Node> children = new List<Node>();
    public Move move;

    public float evaluation;

    public Node(PlayerColor[,] state, PlayerColor turn,Move move)
    {
        this.state=state;
        this.turn = turn;
        this.move=move;
    }

    public Node()
    {
    }

    public List<Node> getChildren()
    {
        return children;
    }
    public void addChildren(Node node)
    {
        this.children.Add(node);
    }
}
