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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TickEffects()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].turns--;
            if (effects[i].effect == Effect.Poisoned)
            {
                effects[i].unit.Damage(5, DTypes.Null, this);
            }
            if (effects[i].turns == 0){
                switch(effects[i].effect)
                {
                    case Effect.Blocking:
                        effects[i].unit.ResetImmunities(new List<DTypes>() { DTypes.Slashing, DTypes.Bludgeoning, DTypes.Piercing });
                        break;
                    case Effect.TrueStrike:
                        effects[i].unit.AddStrength(-30);
                        for (int j = 0; j < 3; j++)
                        {
                            effects[i].unit.GetPhysicalActions().ToArray()[j].ResetDamageType();
                        }
                        break;
                    case Effect.ArmourDown:
                        effects[i].unit.AddPhysDefence(20);
                        break;
                    case Effect.Haste:
                        effects[i].unit.AddSpeed(-15);
                        break;
                    case Effect.MagicWard:
                        effects[i].unit.ResetResistances(new List<DTypes>() { DTypes.Fire, DTypes.Ice, DTypes.Lightning, DTypes.Necrotic, DTypes.Poison, 
                            DTypes.Radiant, DTypes.Holy, DTypes.Wind, DTypes.Arcane });
                        break;
                    case Effect.PoisonBlade:
                        for (int j = 0; j < 3; j++)
                        {
                            effects[i].unit.GetPhysicalActions().ToArray()[j].ResetDamageType();
                        }
                        break;
                    case Effect.Evasion:
                        effects[i].unit.ResetResistances(new List<DTypes>() {DTypes.Slashing, DTypes.Bludgeoning, DTypes.Piercing, DTypes.Fire, DTypes.Ice, 
                            DTypes.Lightning, DTypes.Necrotic, DTypes.Poison, DTypes.Radiant, DTypes.Holy, DTypes.Wind, DTypes.Arcane });
                        break;
                    case Effect.WeaknessMarked:
                        effects[i].unit.ResetVulnerabilities(new List<DTypes>() {DTypes.Slashing, DTypes.Bludgeoning, DTypes.Piercing, DTypes.Fire, DTypes.Ice,
                            DTypes.Lightning, DTypes.Necrotic, DTypes.Poison, DTypes.Radiant, DTypes.Holy, DTypes.Wind, DTypes.Arcane });
                        break;
                    case Effect.Stunned:
                        AddEffect(effects[i].unit, Effect.StunResist, 3);
                        break;
                    case Effect.Ingenuity:
                        effects[i].unit.AddMagicStrength(-50);
                        break;
                }
                effects.RemoveAt(i);
                i--;
            }
        }
    }

    public bool AddEffect(Unit unit, Effect effect, int turns)
    {
        if(effect == Effect.Poisoned)
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

    public void ClearEffects()
    {
        effects.Clear();
    }

    public int CheckForEffect(Unit unit, Effect effect)
    {
        for(int i = 0; i < effects.Count; i++)
        {
            if (effects[i].unit == unit && effects[i].effect == effect)
            {
                return i;
            }
        }
        return -1;
    }
}
