using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWard : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Magic Ward";
        manaCost = 10;
        priority = 0;
        type = DTypes.Radiant;
        originalType = type;
        basePotency = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        if (!th.AddEffect(caster, Effect.MagicWard, 3))
        {
            caster.AddResistance(DTypes.Fire);
            caster.AddResistance(DTypes.Holy);
            caster.AddResistance(DTypes.Ice);
            caster.AddResistance(DTypes.Radiant);
            caster.AddResistance(DTypes.Wind);
            caster.AddResistance(DTypes.Lightning);
            caster.AddResistance(DTypes.Necrotic);
            caster.AddResistance(DTypes.Poison);
            caster.AddResistance(DTypes.Arcane);
        }
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.Buff },
            potency = basePotency,
            effect = Effect.MagicWard
        };
    }
}
