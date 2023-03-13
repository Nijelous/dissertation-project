using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Haste : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Haste";
        manaCost = 10;
        priority = 0;
        type = DTypes.Wind;
        originalType = type;
        basePotency = 15;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        if (!th.AddEffect(caster, Effect.Haste, 5))
        {
            caster.AddSpeed(basePotency);
        }
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.Buff },
            potency = basePotency,
            effect = Effect.Haste
        };
    }
}
