using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Candidate
{
    private int[] actionWeights;
    private int[] typeWeights;

    public Candidate()
    {
        actionWeights = new int[7];
        typeWeights = new int[6];
        for(int i = 0; i < 7; i++)
        {
            actionWeights[i] = Random.Range(0, 100);
        }
        for(int i = 0; i < 6; i++)
        {
            typeWeights[i] = Random.Range(0, 100);
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
        typeWeights = new int[6];
        for (int i = 0; i < 6; i++)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    typeWeights[i] = parent1.GetTypeWeightByInt(i);
                    break;
                case 1:
                    typeWeights[i] = parent2.GetTypeWeightByInt(i);
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
        typeWeights = new int[6];
        for (int i = 0; i < 6; i++)
        {
            switch (Random.Range(0, 6))
            {
                case 0:
                    actionWeights[i] = parent1.GetTypeWeightByInt(i);
                    break;
                case 1:
                    actionWeights[i] = parent2.GetTypeWeightByInt(i);
                    break;
                case 2:
                    actionWeights[i] = parent1.GetTypeWeightByInt(i) + mutationValue;
                    break;
                case 3:
                    actionWeights[i] = parent2.GetTypeWeightByInt(i) + mutationValue;
                    break;
                case 4:
                    actionWeights[i] = parent1.GetTypeWeightByInt(i) - mutationValue;
                    break;
                case 5:
                    actionWeights[i] = parent2.GetTypeWeightByInt(i) - mutationValue;
                    break;
            }
        }
    }

    public Candidate(int[] actionWeights, int[] typeWeights)
    {
        this.actionWeights = actionWeights;
        this.typeWeights = typeWeights;
    }

    public int GetActionWeight(int index)
    {
        if (index < 7) return actionWeights[index];
        else return 0;
    }

    public int GetTypeWeightByInt(int index)
    {
        if (index < 6) return typeWeights[index];
        else return 0;
    }

    public int GetTypeWeightByType(ActionTypes actionType)
    {
        switch (actionType)
        {
            case ActionTypes.Attack: return typeWeights[0];
            case ActionTypes.Healing: return typeWeights[1];
            case ActionTypes.ManaRecovery: return typeWeights[2];
            case ActionTypes.Buff: return typeWeights[3];
            case ActionTypes.Status: return typeWeights[4];
            case ActionTypes.AppliedEffect: return typeWeights[5];
        }
        return 0;
    }

    public string GetWeightsAsString()
    {
        return actionWeights[0] + " " + actionWeights[1] + " " + actionWeights[2] + " " + 
            actionWeights[3] + " " + actionWeights[4] + " " + actionWeights[5] + " " + actionWeights[6];
    }

    public string GetTypeWeightsAsString()
    {
        return typeWeights[0] + " " + typeWeights[1] + " " + typeWeights[2] + " " +
            typeWeights[3] + " " + typeWeights[4] + " " + typeWeights[5];
    }

    public int GetCandidateAction(Unit u, Unit enemy, TurnHandler th)
    {
        int currentAction = 0;
        int currentActionWeight = 0;
        for (int i = 0; i < 7; i++)
        {
            if (i > 2)
            {
                if (u.GetMana() < u.GetMagicalActions().ToArray()[i - 3].GetManaCost())
                {
                    continue;
                }
            }
            int weight = CalculateActionWeight(i > 2 ? u.GetMagicalActions().ToArray()[i - 3] : u.GetPhysicalActions().ToArray()[i],
                u, enemy, GetActionWeight(i), th);
            if (weight > currentActionWeight)
            {
                currentAction = i;
                currentActionWeight = weight;
            }
        }
        return currentAction;
    }
    private int CalculateActionWeight(Action action, Unit u, Unit enemy, int baseWeight, TurnHandler th)
    {
        ActionDetails details = action.ActionEffect();
        int totalWeight = baseWeight;
        foreach (ActionTypes type in details.types)
        {
            switch (type)
            {
                case ActionTypes.Attack:
                    totalWeight += Mathf.RoundToInt(details.potency * ((float)(GetTypeWeightByType(ActionTypes.Attack)) / 100));
                    break;
                case ActionTypes.Healing:
                    totalWeight += Mathf.RoundToInt((1f - u.GetHealthPercentage()) * 100 * details.potency * ((float)(GetTypeWeightByType(ActionTypes.Healing)) / 100));
                    break;
                case ActionTypes.ManaRecovery:
                    totalWeight += Mathf.RoundToInt((1f - u.GetManaPercentage()) * 100 * details.potency * ((float)(GetTypeWeightByType(ActionTypes.ManaRecovery)) / 100));
                    break;
                case ActionTypes.Buff:
                    if (th.CheckForEffect(u, details.effect) == -1)
                    {
                        totalWeight += GetTypeWeightByType(ActionTypes.Buff);
                    }
                    else
                    {
                        totalWeight += GetTypeWeightByType(ActionTypes.AppliedEffect);
                    }
                    break;
                case ActionTypes.Status:
                    if (details.effect == Effect.Stunned && th.CheckForEffect(enemy, Effect.StunResist) != -1)
                    {
                        totalWeight += GetTypeWeightByType(ActionTypes.AppliedEffect);
                    }
                    if (th.CheckForEffect(enemy, details.effect) == -1)
                    {
                        totalWeight += GetTypeWeightByType(ActionTypes.Status);
                    }
                    else
                    {
                        totalWeight += GetTypeWeightByType(ActionTypes.AppliedEffect);
                    }
                    break;
            }
        }
        return totalWeight;
    }
}
