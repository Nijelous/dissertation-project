using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ingenuity : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Ingenuity";
        manaCost = 0;
        priority = 0;
        type = DTypes.Slashing;
        originalType = type;
        basePotency = 50;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        if (!th.AddEffect(caster, Effect.Ingenuity, 2))
        {
            caster.AddMagicStrength(basePotency);
        }
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.Buff },
            potency = basePotency,
            effect = Effect.Ingenuity
        };
    }
}
