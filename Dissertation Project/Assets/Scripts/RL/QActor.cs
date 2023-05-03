using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QActor
{
    private int[] qTable;

    private readonly int actionCount = 7;
    private readonly int turnsRemembered;
    private readonly float healthBarCutoff;
    private readonly float manaBarCutoff;
    private List<float> healthValues;
    private List<float> manaValues;
    private List<float> enemyHealthValues;
    private List<float> enemyManaValues;
    private List<int> actionsUsed;
    private List<int> positiveEffects;
    private List<int> qValuesUsed;

    public QActor(int turnsRemembered, float healthBarCutoff, float manaBarCutoff)
    {
        this.turnsRemembered = turnsRemembered;
        this.healthBarCutoff = healthBarCutoff;
        this.manaBarCutoff = manaBarCutoff;
        healthValues = new();
        manaValues = new();
        enemyHealthValues = new();
        enemyManaValues = new();
        actionsUsed = new();
        positiveEffects = new();
        qValuesUsed = new();
        int tableSize = 0;
        if (turnsRemembered == 0) tableSize = actionCount * (int)(Mathf.Pow(1f / healthBarCutoff, 2) * Mathf.Pow(1f / manaBarCutoff, 2));
        else
        {
            tableSize += actionCount;
            for (int i = 1; i <= turnsRemembered; i++)
            {
                tableSize += (int)(Mathf.Pow(actionCount, i + 1) * Mathf.Pow(1f / healthBarCutoff, 2) * Mathf.Pow(1f / manaBarCutoff, 2));
            }
        }
        qTable = new int[tableSize];
    }

    public QActor(int[] qTable, int turnsRemembered, float healthBarCutoff, float manaBarCutoff)
    {
        this.qTable = qTable;
        this.turnsRemembered = turnsRemembered;
        this.healthBarCutoff = healthBarCutoff;
        this.manaBarCutoff = manaBarCutoff;
    }

    public int GetActorAction(Unit u, Unit enemy, TurnHandler th)
    {
        int currentAction = 0;
        int currentQValue = 0;
        int healthCutoffs = (int)(1f / healthBarCutoff);
        int manaCutoffs = (int)(1f / manaBarCutoff);
        int[] previousActions = th.GetLastActions(u, turnsRemembered);
        int setSelection;
        if (previousActions.Length == 0 && turnsRemembered > 0) setSelection = 0;
        else
        {
            setSelection = ((Mathf.CeilToInt(u.GetHealthPercentage() / healthBarCutoff) - 1) * healthCutoffs * manaCutoffs * manaCutoffs) +
                ((Mathf.CeilToInt(enemy.GetHealthPercentage() / healthBarCutoff) - 1) * manaCutoffs * manaCutoffs) +
                ((u.GetManaPercentage() != 0 ? Mathf.CeilToInt(u.GetManaPercentage() / manaBarCutoff) - 1 : 0) * manaCutoffs) +
                (enemy.GetManaPercentage() != 0 ? Mathf.CeilToInt(enemy.GetManaPercentage() / manaBarCutoff) - 1 : 0);
            if (turnsRemembered > 0) setSelection++;
            for (int i = 1; i < turnsRemembered; i++)
            {
                if (i < previousActions.Length)
                {
                    setSelection += (int)(Mathf.Pow(actionCount, i) * healthCutoffs * healthCutoffs * manaCutoffs * manaCutoffs);
                }
                else
                {
                    for (int j = 0; j < i; j++)
                    {
                        setSelection += previousActions[j] * (int)(Mathf.Pow(actionCount, i - (j + 1)) * healthCutoffs * healthCutoffs * manaCutoffs * manaCutoffs);
                    }
                    break;
                }
            }
        }
        int sectionStart = setSelection * 7;
        if (sectionStart < 0)
        {
            int[] unitActions = th.GetLastActions(u, 16);
            string lastActionsUnit = "";
            for (int i = 0; i < unitActions.Length; i++)
            {
                lastActionsUnit += unitActions[i] < 3 ? u.GetPhysicalActions().ToArray()[unitActions[i]].GetActionName() : u.GetMagicalActions().ToArray()[unitActions[i] - 3].GetActionName();
                lastActionsUnit += " ";
            }
            int[] enemyActions = th.GetLastActions(enemy, 16);
            string lastActionsEnemy = "";
            for (int i = 0; i < enemyActions.Length; i++)
            {
                lastActionsEnemy += unitActions[i] < 3 ? enemy.GetPhysicalActions().ToArray()[unitActions[i]].GetActionName() : enemy.GetMagicalActions().ToArray()[unitActions[i] - 3].GetActionName();
                lastActionsEnemy += " ";
            }
            Debug.LogError("Unit Name: " + u.GetUnitName() + " Section Start: " + sectionStart + " Table Length: " + qTable.Length + "\nHealth: " + u.GetHealthPercentage() + " Enemy Health: " + enemy.GetHealthPercentage() +
                "\nMana: " + u.GetManaPercentage() + " Enemy Mana: " + enemy.GetManaPercentage() + "\nUnit Actions: " + lastActionsUnit + "\nEnemy Actions: " + lastActionsEnemy);
        }
        for (int i = 0; i < 7; i++)
        {
            int qValue = qTable[sectionStart + i];
            if (qValue > currentQValue)
            {
                currentAction = i;
                currentQValue = qValue;
            }
        }
        return currentAction;
    }

    public int GetActorActionLearning(Unit u, Unit enemy, TurnHandler th, bool exploring)
    {
        int currentAction = 0;
        int currentQValue = 0;
        int healthCutoffs = (int)(1f / healthBarCutoff);
        int manaCutoffs = (int)(1f / manaBarCutoff);
        int[] previousActions = th.GetLastActions(u, turnsRemembered);
        healthValues.Add(u.GetHealthPercentage());
        manaValues.Add(u.GetManaPercentage());
        enemyHealthValues.Add(enemy.GetHealthPercentage());
        enemyManaValues.Add(enemy.GetManaPercentage());
        positiveEffects.Add(th.GetPositiveEffects(u, enemy));
        int setSelection;
        if (previousActions.Length == 0 && turnsRemembered > 0) setSelection = 0;
        else
        {
            setSelection = ((Mathf.CeilToInt(u.GetHealthPercentage() / healthBarCutoff) - 1) * healthCutoffs * manaCutoffs * manaCutoffs) +
                ((Mathf.CeilToInt(enemy.GetHealthPercentage() / healthBarCutoff) - 1) * manaCutoffs * manaCutoffs) +
                ((u.GetManaPercentage() != 0 ? Mathf.CeilToInt(u.GetManaPercentage() / manaBarCutoff) - 1 : 0) * manaCutoffs) +
                (enemy.GetManaPercentage() != 0 ? Mathf.CeilToInt(enemy.GetManaPercentage() / manaBarCutoff) - 1 : 0);
            if (turnsRemembered > 0) setSelection++;
            for (int i = 1; i < turnsRemembered; i++)
            {
                if (i < previousActions.Length)
                {
                    setSelection += (int)(Mathf.Pow(actionCount, i) * healthCutoffs * healthCutoffs * manaCutoffs * manaCutoffs);
                }
                else
                {
                    for (int j = 0; j < i; j++)
                    {
                        setSelection += previousActions[j] * (int)(Mathf.Pow(actionCount, i - (j + 1)) * healthCutoffs * healthCutoffs * manaCutoffs * manaCutoffs);
                    }
                    break;
                }
            }
        }
        int sectionStart = setSelection * 7;
        if (sectionStart < 0)
        {
            int[] unitActions = th.GetLastActions(u, 16);
            string lastActionsUnit = "";
            for (int i = 0; i < unitActions.Length; i++)
            {
                lastActionsUnit += unitActions[i] < 3 ? u.GetPhysicalActions().ToArray()[unitActions[i]].GetActionName() : u.GetMagicalActions().ToArray()[unitActions[i] - 3].GetActionName();
                lastActionsUnit += " ";
            }
            int[] enemyActions = th.GetLastActions(enemy, 16);
            string lastActionsEnemy = "";
            for (int i = 0; i < enemyActions.Length; i++)
            {
                lastActionsEnemy += unitActions[i] < 3 ? enemy.GetPhysicalActions().ToArray()[unitActions[i]].GetActionName() : enemy.GetMagicalActions().ToArray()[unitActions[i] - 3].GetActionName();
                lastActionsEnemy += " ";
            }
            Debug.LogError("Unit Name: " + u.GetUnitName() + " Section Start: " + sectionStart + " Table Length: " + qTable.Length + "\nHealth: " + u.GetHealthPercentage() + " Enemy Health: " + enemy.GetHealthPercentage() +
                "\nMana: " + u.GetManaPercentage() + " Enemy Mana: " + enemy.GetManaPercentage() + "\nUnit Actions: " + lastActionsUnit + "\nEnemy Actions: " + lastActionsEnemy);
        }
        if (exploring)
        {
            bool validAction = false;
            while (!validAction)
            {
                currentAction = Random.Range(0, 7);
                if (currentAction > 2)
                {
                    if (u.GetMana() >= u.GetMagicalActions().ToArray()[currentAction - 3].GetManaCost())
                    {
                        validAction = true;
                    }
                }
                else
                {
                    validAction = true;
                }
            }
        }
        else
        {
            for (int i = 0; i < 7; i++)
            {
                if (i > 2)
                {
                    if (u.GetMana() < u.GetMagicalActions().ToArray()[i - 3].GetManaCost())
                    {
                        qTable[sectionStart + i] = -100;
                        continue;
                    }
                }
                int qValue = qTable[sectionStart + i];
                if (qValue > currentQValue)
                {
                    currentAction = i;
                    currentQValue = qValue;
                }
            }
        }
        actionsUsed.Add(currentAction);
        qValuesUsed.Add(sectionStart + currentAction);
        return currentAction;
    }

    public void UpdateQTable(Unit u, Unit enemy, float learningRate, float discountRate)
    {
        healthValues.Add(u.GetHealthPercentage());
        manaValues.Add(u.GetManaPercentage());
        enemyHealthValues.Add(enemy.GetHealthPercentage());
        enemyManaValues.Add(enemy.GetManaPercentage());
        positiveEffects.Add(0);
        for(int i = qValuesUsed.Count-1; i >= 0; i--)
        {
            /*Debug.Log("qValue: " + qValuesUsed[i] + 
                "\nEnemy Health: " + enemyHealthValues[i] + " || Enemy Health After: " + enemyHealthValues[i + 1] +
                "\nEnemy Mana: " + enemyManaValues[i] + " || Enemy Mana After: " + enemyManaValues[i + 1] +
                "\nHealth: " + healthValues[i] + " || Health After: " + healthValues[i + 1] +
                "\nMana: " + manaValues[i] + " || Mana After: " + manaValues[i + 1]);*/
            float oldValue = (1 - learningRate)*qTable[qValuesUsed[i]];
            float reward = (enemyHealthValues[i] - enemyHealthValues[i + 1]) * 100 - (healthValues[i] - healthValues[i + 1]) * 100 
                + (enemyManaValues[i] - enemyManaValues[i + 1]) * 25 - (manaValues[i] - manaValues[i + 1]) * 25 + positiveEffects[i+1] * 25;
            float nextValue = 0;
            if (enemyHealthValues[i + 1] == 0) nextValue = 300;
            else if (healthValues[i + 1] == 0) nextValue = -enemy.GetHealth();
            else
            {
                for (int j = 0; j < 7; j++)
                {
                    float value = qTable[qValuesUsed[i + 1] - actionsUsed[i + 1] + j];
                    if (value > nextValue) nextValue = value;
                }
            }
            if (i >= 1 && healthValues[i] > healthValues[i + 1] && enemyHealthValues[i] <= enemyHealthValues[i + 1] && actionsUsed[i - 1] == actionsUsed[i])
            {
                /*Debug.Log("Actions: " + actionsUsed[i-1] + " " + actionsUsed[i] + 
                    "\nEnemy Health: " + enemyHealthValues[i] + " || Enemy Health After: " + enemyHealthValues[i + 1] +
                    "\nEnemy Mana: " + enemyManaValues[i] + " || Enemy Mana After: " + enemyManaValues[i + 1] +
                    "\nHealth: " + healthValues[i] + " || Health After: " + healthValues[i + 1] +
                    "\nMana: " + manaValues[i] + " || Mana After: " + manaValues[i + 1]);*/
                nextValue -= 100;
            }
            
            qTable[qValuesUsed[i]] = (int)(oldValue + (learningRate * (reward + (discountRate * nextValue))));
        }
        healthValues.Clear();
        manaValues.Clear();
        enemyHealthValues.Clear();
        enemyManaValues.Clear();
        actionsUsed.Clear();
        positiveEffects.Clear();
        qValuesUsed.Clear();
    }

    public void PrintQTable()
    {
        string s = "";
        for(int i = 0; i < qTable.Length; i++)
        {
            if (i % 49 == 0) s += "\n";
            else if (i % 7 == 0) s += " || ";
            s += "" + qTable[i] + " ";
        }
        Debug.Log(s);
    }

    public string GetQTable()
    {
        string s = "";
        for(int i = 0; i < qTable.Length; i++)
        {
            s += "" + qTable[i] + " ";
        }
        return s;
    }
}
