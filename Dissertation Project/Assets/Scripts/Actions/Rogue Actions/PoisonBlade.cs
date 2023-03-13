using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoisonBlade : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Poison Blade";
        manaCost = 5;
        priority = 0;
        type = DTypes.Poison;
        originalType = type;
        basePotency = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        if (!th.AddEffect(caster, Effect.PoisonBlade, 5))
        {
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
            effect = Effect.PoisonBlade
        };
    }
}
