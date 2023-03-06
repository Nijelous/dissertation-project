using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candidate
{
    private int[] actionWeights;

    public Candidate()
    {
        actionWeights = new int[7];
        for(int i = 0; i < 7; i++)
        {
            actionWeights[i] = Random.Range(0, 100);
        }
    }

    public Candidate(Candidate parent1, Candidate parent2)
    {
        actionWeights = new int[7];
        for (int i = 0; i < 7; i++)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    actionWeights[i] = parent1.GetActionWeight(i);
                    break;
                case 1:
                    actionWeights[i] = parent2.GetActionWeight(i);
                    break;
            }
        }
    }

    public Candidate(Candidate parent1, Candidate parent2, int mutationValue)
    {
        actionWeights = new int[7];
        for(int i = 0; i < 7; i++)
        {
            switch(Random.Range(0, 6))
            {
                case 0:
                    actionWeights[i] = parent1.GetActionWeight(i);
                    break;
                case 1:
                    actionWeights[i] = parent2.GetActionWeight(i);
                    break;
                case 2:
                    actionWeights[i] = parent1.GetActionWeight(i) + mutationValue;
                    break;
                case 3:
                    actionWeights[i] = parent2.GetActionWeight(i) + mutationValue;
                    break;
                case 4:
                    actionWeights[i] = parent1.GetActionWeight(i) - mutationValue;
                    break;
                case 5:
                    actionWeights[i] = parent2.GetActionWeight(i) - mutationValue;
                    break;
            }
        }
    }

    public int GetActionWeight(int index)
    {
        if (index < 7) return actionWeights[index];
        else return 0;
    }

    public string GetWeightsAsString()
    {
        return actionWeights[0] + " " + actionWeights[1] + " " + actionWeights[2] + " " + 
            actionWeights[3] + " " + actionWeights[4] + " " + actionWeights[5] + " " + actionWeights[6];
    }
}
