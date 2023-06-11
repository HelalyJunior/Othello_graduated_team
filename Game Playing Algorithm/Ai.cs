using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai
{
    int depth;
    PlayerColor player;

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


}
