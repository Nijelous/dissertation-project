using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrueStrike : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "True Strike";
        manaCost = 5;
        priority = 0;
        type = DTypes.Radiant;
        originalType = type;
        basePotency = 30;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        if (!th.AddEffect(caster, Effect.TrueStrike, 3))
        {
            caster.AddStrength(basePotency);
            for (int i = 0; i < 3; i++)
            {
                caster.GetPhysicalActions().ToArray()[i].SetDamageType(type);
            }
        }
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.Buff },
            potency = basePotency,
            effect = Effect.TrueStrike
        };
    }
}
