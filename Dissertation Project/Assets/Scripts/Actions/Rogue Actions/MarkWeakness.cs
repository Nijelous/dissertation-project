using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkWeakness : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Mark Weakness";
        manaCost = 25;
        priority = 0;
        type = DTypes.Wind;
        originalType = type;
        basePotency = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        if (!th.AddEffect(caster, Effect.WeaknessMarked, 3))
        {
            caster.AddVulnerability(DTypes.Bludgeoning);
            caster.AddVulnerability(DTypes.Slashing);
            caster.AddVulnerability(DTypes.Piercing);
            caster.AddVulnerability(DTypes.Fire);
            caster.AddVulnerability(DTypes.Holy);
            caster.AddVulnerability(DTypes.Ice);
            caster.AddVulnerability(DTypes.Radiant);
            caster.AddVulnerability(DTypes.Wind);
            caster.AddVulnerability(DTypes.Lightning);
            caster.AddVulnerability(DTypes.Necrotic);
            caster.AddVulnerability(DTypes.Poison);
            caster.AddVulnerability(DTypes.Arcane);
        }
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.Status },
            potency = basePotency,
            effect = Effect.WeaknessMarked
        };
    }

}
