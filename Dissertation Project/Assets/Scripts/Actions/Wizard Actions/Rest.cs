using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rest : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Rest";
        manaCost = 0;
        priority = 0;
        type = DTypes.Bludgeoning;
        originalType = type;
        basePotency = 5;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        caster.AddHealth(basePotency + (caster.GetMagicStrength() / 10));
        caster.AddMana(basePotency + (caster.GetMagicStrength() / 10));
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.Healing, ActionTypes.ManaRecovery },
            potency = basePotency,
            effect = Effect.Null
        };
    }
}
