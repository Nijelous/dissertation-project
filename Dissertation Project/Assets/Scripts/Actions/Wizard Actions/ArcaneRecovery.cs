using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneRecovery : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Arcane Recovery";
        manaCost = 10;
        priority = 0;
        type = DTypes.Arcane;
        originalType = type;
        basePotency = 40;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        caster.AddMana(basePotency + (caster.GetMagicStrength() / 10));
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.ManaRecovery },
            potency = basePotency,
            effect = Effect.Null
        };
    }
}
