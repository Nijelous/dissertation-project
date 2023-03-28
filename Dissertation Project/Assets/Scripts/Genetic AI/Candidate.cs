using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CandidateType
{
    Basic,
    Advanced,
    AdvancedHeuristic
}
public class Candidate
{
    private int[] weights;

    private readonly int actionCount = 7;
    private readonly int turnsRemembered;
    private readonly float healthBarCutoff;
    private readonly float manaBarCutoff;
    private readonly int maxWeight = 99;
    private List<int> sectionsUsed;

    public Candidate(CandidateType ct, int turnsRemembered, float healthBarCutoff, float manaBarCutoff)
    {
        this.turnsRemembered = turnsRemembered;
        this.healthBarCutoff = healthBarCutoff;
        this.manaBarCutoff = manaBarCutoff;
        int weightCount = 0;
        switch (ct)
        {
            case CandidateType.Basic:
                weights = new int[13];
                break;
            case CandidateType.Advanced:
                if (turnsRemembered == 0) weightCount = actionCount * (int)(Mathf.Pow(1f / healthBarCutoff, 2) * Mathf.Pow(1f / manaBarCutoff, 2));
                else {
                    weightCount += actionCount;
                    for (int i = 1; i <= turnsRemembered; i++)
                    {
                        weightCount += (int)(Mathf.Pow(actionCount, i+1) * Mathf.Pow(1f / healthBarCutoff, 2) * Mathf.Pow(1f / manaBarCutoff, 2));
                    }
                }
                weights = new int[weightCount];
                break;
            case CandidateType.AdvancedHeuristic:
                if (turnsRemembered == 0) weightCount = actionCount * (int)(Mathf.Pow(1f / healthBarCutoff, 2) * Mathf.Pow(1f / manaBarCutoff, 2));
                else
                {
                    weightCount += actionCount;
                    for (int i = 1; i <= turnsRemembered; i++)
                    {
                        weightCount += (int)(Mathf.Pow(actionCount, i + 1) * Mathf.Pow(1f / healthBarCutoff, 2) * Mathf.Pow(1f / manaBarCutoff, 2));
                    }
                }
                weights = new int[weightCount];
                sectionsUsed = new List<int>();
                break;
        }
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Random.Range(0, maxWeight+1);
        }
    }

    public Candidate(Candidate parent1, Candidate parent2)
    {
        turnsRemembered = parent1.turnsRemembered;
        healthBarCutoff = parent1.healthBarCutoff;
        manaBarCutoff = parent1.manaBarCutoff;
        if (parent1.sectionsUsed == null)
        {
            weights = new int[parent1.weights.Length];
            for (int i = 0; i < weights.Length; i++)
            {
                switch (Random.Range(0, 2))
                {
                    case 0:
                        weights[i] = parent1.weights[i];
                        break;
                    case 1:
                        weights[i] = parent2.weights[i];
                        break;
                }
            }
        }
        else
        {
            weights = parent1.weights;
            sectionsUsed = new List<int>();
            HashSet<int> sections = new();
            foreach (int i in parent1.sectionsUsed)
            {
                sections.Add(i);
            }
            foreach (int i in parent2.sectionsUsed)
            {
                sections.Add(i);
            }
            foreach (int sectionStart in sections)
            {
                for(int i = 0; i < actionCount; i++)
                {
                    switch (Random.Range(0, 2))
                    {
                        case 0:
                            weights[sectionStart + i] = parent1.weights[sectionStart + i];
                            break;
                        case 1:
                            weights[sectionStart + i] = parent2.weights[sectionStart + i];
                            break;
                    }
                }
            }
        }
    }

    public Candidate(Candidate parent1, Candidate parent2, int mutationValue)
    {
        turnsRemembered = parent1.turnsRemembered;
        healthBarCutoff = parent1.healthBarCutoff;
        manaBarCutoff = parent1.manaBarCutoff;
        if (parent1.sectionsUsed == null)
        {
            weights = new int[parent1.weights.Length];
            for (int i = 0; i < weights.Length; i++)
            {
                switch (Random.Range(0, 6))
                {
                    case 0:
                        weights[i] = parent1.weights[i];
                        break;
                    case 1:
                        weights[i] = parent2.weights[i];
                        break;
                    case 2:
                        weights[i] = Mathf.Min(parent1.weights[i] + mutationValue, maxWeight);
                        break;
                    case 3:
                        weights[i] = Mathf.Min(parent2.weights[i] + mutationValue, maxWeight);
                        break;
                    case 4:
                        weights[i] = Mathf.Max(parent1.weights[i] - mutationValue, 0);
                        break;
                    case 5:
                        weights[i] = Mathf.Max(parent2.weights[i] - mutationValue, 0);
                        break;
                }
            }
        }
        else
        {
            weights = parent1.weights;
            sectionsUsed = new List<int>();
            HashSet<int> sections = new();
            foreach (int i in parent1.sectionsUsed)
            {
                sections.Add(i);
            }
            foreach (int i in parent2.sectionsUsed)
            {
                sections.Add(i);
            }
            foreach (int sectionStart in sections)
            {
                for (int i = 0; i < actionCount; i++)
                {
                    switch (Random.Range(0, 2))
                    {
                        case 0:
                            weights[sectionStart + i] = parent1.weights[sectionStart + i];
                            break;
                        case 1:
                            weights[sectionStart + i] = parent2.weights[sectionStart + i];
                            break;
                        case 2:
                            weights[sectionStart + i] = Mathf.Min(parent1.weights[sectionStart + i] + mutationValue, maxWeight);
                            break;
                        case 3:
                            weights[sectionStart + i] = Mathf.Min(parent2.weights[sectionStart + i] + mutationValue, maxWeight);
                            break;
                        case 4:
                            weights[sectionStart + i] = Mathf.Max(parent1.weights[sectionStart + i] - mutationValue, 0);
                            break;
                        case 5:
                            weights[sectionStart + i] = Mathf.Max(parent2.weights[sectionStart + i] - mutationValue, 0);
                            break;
                    }
                }
            }
        }
    }

    public Candidate(int[] weights, int turnsRemembered, float healthBarCutoff, float manaBarCutoff)
    {
        this.weights = weights;
        this.turnsRemembered = turnsRemembered;
        this.healthBarCutoff = healthBarCutoff;
        this.manaBarCutoff = manaBarCutoff;
    }

    public int GetWeight(int index)
    {
        return weights[index];
    }

    public int[] GetWeights()
    {
        return weights;
    }

    public int GetActionWeight(int index)
    {
        if (index < 7) return weights[index];
        else return 0;
    }

    public int GetTypeWeightByInt(int index)
    {
        if (index < 6) return weights[index+7];
        else return 0;
    }

    public int GetTypeWeightByType(ActionTypes actionType)
    {
        switch (actionType)
        {
            case ActionTypes.Attack: return weights[7];
            case ActionTypes.Healing: return weights[8];
            case ActionTypes.ManaRecovery: return weights[9];
            case ActionTypes.Buff: return weights[10];
            case ActionTypes.Status: return weights[11];
            case ActionTypes.AppliedEffect: return weights[12];
        }
        return 0;
    }

    public string GetWeightsAsString()
    {
        return weights[0] + " " + weights[1] + " " + weights[2] + " " + weights[3] + " " + weights[4] + " " + weights[5] + " " + weights[6];
    }

    public string GetTypeWeightsAsString()
    {
        return weights[7] + " " + weights[8] + " " + weights[9] + " " + weights[10] + " " + weights[11] + " " + weights[12];
    }

    public int GetWeightsLength()
    {
        return weights.Length;
    }

    public int GetCandidateAction(Unit u, Unit enemy, TurnHandler th)
    {
        if (weights.Length == 13)
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
        else
        {
            int currentAction = 0;
            int currentActionWeight = 0;
            int healthCutoffs = (int)(1f / healthBarCutoff);
            int manaCutoffs = (int)(1f / manaBarCutoff);
            int[] previousActions = th.GetLastActions(u, turnsRemembered);
            int geneSelection;
            if (previousActions.Length == 0 && turnsRemembered > 0) geneSelection = 0;
            else
            {
                geneSelection = ((Mathf.CeilToInt(u.GetHealthPercentage() / healthBarCutoff) - 1) * healthCutoffs * manaCutoffs * manaCutoffs) +
                    ((Mathf.CeilToInt(enemy.GetHealthPercentage() / healthBarCutoff) - 1) * manaCutoffs * manaCutoffs) +
                    ((u.GetManaPercentage() != 0 ? Mathf.CeilToInt(u.GetManaPercentage() / manaBarCutoff) - 1 : 0) * manaCutoffs) +
                    (enemy.GetManaPercentage() != 0 ? Mathf.CeilToInt(enemy.GetManaPercentage() / manaBarCutoff) - 1 : 0);
                if (turnsRemembered > 0) geneSelection++;
                for (int i = 1; i < turnsRemembered; i++)
                {
                    if (i < previousActions.Length)
                    {
                        geneSelection += (int)(Mathf.Pow(actionCount, i) * healthCutoffs * healthCutoffs * manaCutoffs * manaCutoffs);
                    }
                    else
                    {
                        for (int j = 0; j < i; j++)
                        {
                            geneSelection += previousActions[j] * (int)(Mathf.Pow(actionCount, i - (j+1)) * healthCutoffs * healthCutoffs * manaCutoffs * manaCutoffs);
                        }
                        break;
                    }
                }
            }
            int sectionStart = geneSelection * 7;
            sectionsUsed?.Add(sectionStart);
            if (sectionStart < 0)
            {
                int[] unitActions = th.GetLastActions(u, 16);
                string lastActionsUnit = "";
                for(int i = 0; i < unitActions.Length; i++)
                {
                    lastActionsUnit += unitActions[i] < 3 ? u.GetPhysicalActions().ToArray()[unitActions[i]].GetActionName() : u.GetMagicalActions().ToArray()[unitActions[i]-3].GetActionName();
                    lastActionsUnit += " ";
                }
                int[] enemyActions = th.GetLastActions(enemy, 16);
                string lastActionsEnemy = "";
                for (int i = 0; i < enemyActions.Length; i++)
                {
                    lastActionsEnemy += unitActions[i] < 3 ? enemy.GetPhysicalActions().ToArray()[unitActions[i]].GetActionName() : enemy.GetMagicalActions().ToArray()[unitActions[i]-3].GetActionName();
                    lastActionsEnemy += " ";
                }
                Debug.LogError("Unit Name: " + u.GetUnitName() + " Section Start: " + sectionStart + " Weights: " + weights.Length + "\nHealth: " + u.GetHealthPercentage() + " Enemy Health: " + enemy.GetHealthPercentage() +
                    "\nMana: " + u.GetManaPercentage() + " Enemy Mana: " + enemy.GetManaPercentage() + "\nUnit Actions: " + lastActionsUnit + "\nEnemy Actions: " + lastActionsEnemy);
            }
            for (int i = 0; i < 7; i++)
            {
                if(i > 2)
                {
                    if (u.GetMana() < u.GetMagicalActions().ToArray()[i - 3].GetManaCost())
                    {
                        continue;
                    }
                }
                int weight = weights[sectionStart + i];
                if(weight > currentActionWeight)
                {
                    currentAction = i;
                    currentActionWeight = weight;
                }
            }
            return currentAction;
        }
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
