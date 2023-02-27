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
    public override void Act(Unit caster, Unit enemy)
    {
        caster.AddImmunity(DTypes.Bludgeoning);
        caster.AddImmunity(DTypes.Slashing);
        caster.AddImmunity(DTypes.Piercing);
        GameObject.Find("TurnHandler").GetComponent<TurnHandler>().AddEffect(caster, Effect.Blocking, 1);
    }

    public override void ActionEffect(Unit caster, Unit enemy)
    {
        throw new System.NotImplementedException();
    }
}
