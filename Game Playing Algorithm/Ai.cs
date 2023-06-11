using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai
{
    int depth;
    PlayerColor player;

    public Node generateTree(PlayerColor[,] state)
    {
        return generateGameTree(state,player,depth,new Node(state,player,new Move()));
    }

    private Node generateGameTree(PlayerColor[,] state, PlayerColor player, int depth,Node root)
    {
        if(depth==0 || GameLogic.isGameOverGivenState(state))
        {
            return new Node();
        }
        Dictionary<Position,List<Position>> legalPositions = GameLogic.getLegalMovesGivenState(state,player);
        foreach(KeyValuePair<Position,List<Position>> legalPos in legalPositions)
        {
            Move move = new Move(player,legalPos.Value,legalPos.Key);
            PlayerColor[,] childState = GameLogic.takeMoveGivenState(state
            ,move,player,out PlayerColor turn);
            Node child = new Node(childState,turn,move);
            root.addChildren(child);
            generateGameTree(childState,turn,depth-1,child);
        }

        return root;
    }

    public Ai(int depth , PlayerColor player)
    {
        this.depth=depth;
        this.player = player;
    }

    private int evaluateState(Node stateNode)
    {
        return 10*evaluateCoinParity(stateNode)+
        100*evaluateActualMobility(stateNode)+
        1000*evaluateCornersCaptured(stateNode);
    } 
    private int evaluateCornersCaptured(Node stateNode)
    {
        int maxCount=0;
        int minCount=0;

        int[]i = new int[]{0,7};
        int[]j = new int[]{0,7};

        foreach(int ii in i)
        {
            foreach(int jj in j)
            {
                if(stateNode.state[ii,jj]==player)
                {
                    maxCount++;
                }
                else if(stateNode.state[ii,jj]==Player.getOpponent(player))
                {
                    minCount++;
                }
            }
        }
        if(maxCount+minCount==0)
        {
            return 0;
        }
        return 100* ((maxCount-minCount)/(maxCount+minCount));
    }
    private int evaluateActualMobility(Node stateNode)
    {
        int maxCount= GameLogic.getLegalMovesGivenState(stateNode.state,player).Count;
        int minCount=GameLogic.getLegalMovesGivenState(stateNode.state,Player.getOpponent(player)).Count;

        if(maxCount+minCount ==0)
        {
            return 0;
        }

        return 100*((maxCount-minCount)/(maxCount+minCount));
    }
    private int evaluateCoinParity(Node stateNode)
    {
        int maxCount=0;
        int minCount=0;

        for(int i=0;i<8;i++)
        {
            for(int j=0;j<8;j++)
            {
                if(stateNode.state[i,j]==player)
                {
                    maxCount++;
                }
                else if(stateNode.state[i,j]==Player.getOpponent(player))
                {
                    minCount++;
                }
            }
        }

        if(maxCount+minCount ==0)
        {
            return 0;
        }

        return 100* ((maxCount-minCount)/(maxCount+minCount));
    }

    private float minMaxAlphaBetaPruning(Node gameTree , float alpha , float beta)
    {
        if(gameTree.getChildren().Count==0)
        {
            return evaluateState(gameTree);
        }

        if(gameTree.turn==player) //maximizer
        {
            float maxEvaluation= Mathf.NegativeInfinity;
            foreach(Node child in gameTree.getChildren())
            {
                float evaluation = minMaxAlphaBetaPruning(child,alpha,beta);
                child.evaluation = evaluation;
                maxEvaluation=Mathf.Max(maxEvaluation,evaluation);
                alpha = Mathf.Max(alpha,evaluation);
                if(beta<=alpha)
                {
                    break;
                }
            }
            return maxEvaluation;

        }
        else //minimizer
        {
            float minEvaluation=Mathf.Infinity;
            foreach(Node child in gameTree.getChildren())
            {
                float evaluation = minMaxAlphaBetaPruning(child,alpha,beta);
                child.evaluation = evaluation;
                minEvaluation=Mathf.Min(minEvaluation,evaluation);
                beta = Mathf.Min(beta,evaluation);
                if(beta<=alpha)
                {
                    break;
                }
            }
            return minEvaluation;
        }
    }

    public Move getBestMove(PlayerColor[,] state)
    {
        Node gameTree = generateTree(state);
        float bestScore = minMaxAlphaBetaPruning(gameTree,Mathf.NegativeInfinity,Mathf.Infinity);
        foreach(Node child in gameTree.getChildren())
        {
            if(child.evaluation==bestScore)
            {
                return child.move;
            }
        }
        return new Move();
    }




}
