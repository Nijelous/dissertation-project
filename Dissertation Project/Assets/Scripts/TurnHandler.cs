using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Effect
{
    Blocking,
    TrueStrike,
    ArmourDown,
    Haste,
    MagicWard,
    PoisonBlade,
    Poisoned,
    Evasion,
    WeaknessMarked,
    Stunned,
    StunResist,
    Ingenuity,
    Null
}
public class UnitEffect
{
    public Unit unit;
    public Effect effect;
    public int turns;
}
public class TurnHandler : MonoBehaviour
{
    private List<UnitEffect> effects = new();
    private Dictionary<Unit, List<int>> lastActionUsed = new();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TickEffects(Unit p1, Unit p2, int action1, int action2)
    {
        if (lastActionUsed.Count == 0)
        {
            List<int> list1 = new List<int> { action1 };
            List<int> list2 = new List<int> { action2 };
            lastActionUsed.Add(p1, list1);
            lastActionUsed.Add(p2, list2);
        }
        else
        {
            lastActionUsed[p1].Add(action1);
            lastActionUsed[p2].Add(action2);
        }
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].turns--;
            if (effects[i].effect == Effect.Poisoned)
            {
                effects[i].unit.Damage(5, DTypes.Null, this);
            }
            if (effects[i].turns == 0)
            {
                ResetEffect(effects[i]);
                effects.RemoveAt(i);
                i--;
            }
        }
    }

    public bool AddEffect(Unit unit, Effect effect, int turns)
    {
        if (effect == Effect.Poisoned)
        {
            UnitEffect newEffect = new()
            {
                unit = unit,
                effect = effect,
                turns = turns
            };
            effects.Add(newEffect);
            return false;
        }
        int index = CheckForEffect(unit, effect);
        if (index == -1)
        {
            UnitEffect newEffect = new()
            {
                unit = unit,
                effect = effect,
                turns = turns
            };
            effects.Add(newEffect);
            return false;
        }
        else
        {
            effects[index].turns = turns;
            return true;
        }
    }

    private void ResetEffect(UnitEffect effect)
    {
        switch (effect.effect)
        {
            case Effect.Blocking:
                effect.unit.ResetImmunities(new List<DTypes>() { DTypes.Slashing, DTypes.Bludgeoning, DTypes.Piercing });
                break;
            case Effect.TrueStrike:
                effect.unit.AddStrength(-30);
                for (int j = 0; j < 3; j++)
                {
                    effect.unit.GetPhysicalActions().ToArray()[j].ResetDamageType();
                }
                break;
            case Effect.ArmourDown:
                effect.unit.AddPhysDefence(20);
                break;
            case Effect.Haste:
                effect.unit.AddSpeed(-15);
                break;
            case Effect.MagicWard:
                effect.unit.ResetResistances(new List<DTypes>() { DTypes.Fire, DTypes.Ice, DTypes.Lightning, DTypes.Necrotic, DTypes.Poison,
                            DTypes.Radiant, DTypes.Holy, DTypes.Wind, DTypes.Arcane });
                break;
            case Effect.PoisonBlade:
                for (int j = 0; j < 3; j++)
                {
                    effect.unit.GetPhysicalActions().ToArray()[j].ResetDamageType();
                }
                break;
            case Effect.Evasion:
                effect.unit.ResetResistances(new List<DTypes>() {DTypes.Slashing, DTypes.Bludgeoning, DTypes.Piercing, DTypes.Fire, DTypes.Ice,
                            DTypes.Lightning, DTypes.Necrotic, DTypes.Poison, DTypes.Radiant, DTypes.Holy, DTypes.Wind, DTypes.Arcane });
                break;
            case Effect.WeaknessMarked:
                effect.unit.ResetVulnerabilities(new List<DTypes>() {DTypes.Slashing, DTypes.Bludgeoning, DTypes.Piercing, DTypes.Fire, DTypes.Ice,
                            DTypes.Lightning, DTypes.Necrotic, DTypes.Poison, DTypes.Radiant, DTypes.Holy, DTypes.Wind, DTypes.Arcane });
                break;
            case Effect.Stunned:
                AddEffect(effect.unit, Effect.StunResist, 3);
                break;
            case Effect.Ingenuity:
                effect.unit.AddMagicStrength(-50);
                break;
        }
    }

    public void ClearEffects()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            ResetEffect(effects[i]);
            effects.RemoveAt(i);
            i--;
        }
        lastActionUsed.Clear();
    }

    public int CheckForEffect(Unit unit, Effect effect)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].unit == unit && effects[i].effect == effect)
            {
                return i;
            }
        }
        return -1;
    }

    public int[] GetLastActions(Unit unit, int turnsBack)
    {
        if (!lastActionUsed.ContainsKey(unit)) return new int[0];
        else
        {
            int[] result = new int[Mathf.Min(turnsBack, lastActionUsed[unit].Count)];
            for(int i = 0; i < result.Length; i++)
            {
                result[i] = lastActionUsed[unit][lastActionUsed[unit].Count-i-1];
            }
            return result;
        }
    }
}
