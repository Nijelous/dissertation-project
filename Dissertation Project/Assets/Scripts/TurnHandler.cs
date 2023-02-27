using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Effect
{
    Blocking,
    TrueStrike
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
            if (effects[i].turns == 0){
                switch(effects[i].effect)
                {
                    case Effect.Blocking:
                        effects[i].unit.RemoveImmunity(DTypes.Slashing);
                        effects[i].unit.RemoveImmunity(DTypes.Bludgeoning);
                        effects[i].unit.RemoveImmunity(DTypes.Piercing);
                        break;
                    case Effect.TrueStrike:
                        effects[i].unit.AddStrength(-30);
                        for (int j = 0; j < 3; j++)
                        {
                            effects[i].unit.GetPhysicalActions().ToArray()[j].ResetDamageType();
                        }
                        break;
                }
                effects.RemoveAt(i);
                i--;
            }
        }
    }

    public void AddEffect(Unit unit, Effect effect, int turns)
    {
        UnitEffect newEffect = new()
        {
            unit = unit,
            effect = effect,
            turns = turns
        };
        effects.Add(newEffect);
    }

    public void ClearEffects()
    {
        effects.Clear();
    }
}
