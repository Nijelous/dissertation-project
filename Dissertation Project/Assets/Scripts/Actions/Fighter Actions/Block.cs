using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Block";
        manaCost = 0;
        priority = 3;
        type = DTypes.Bludgeoning;
        originalType = type;
        basePotency = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        if (!th.AddEffect(caster, Effect.Blocking, 1))
        {
            caster.AddImmunity(DTypes.Bludgeoning);
            caster.AddImmunity(DTypes.Slashing);
            caster.AddImmunity(DTypes.Piercing);
        }
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.Buff },
            potency = basePotency,
            effect = Effect.Blocking
        };
    }
}
