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
        basePotency = 50;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Act(Unit caster, Unit enemy)
    {
        caster.AddStrength(30);
        for(int i = 0; i < 3; i++)
        {
            caster.GetPhysicalActions().ToArray()[i].SetDamageType(type);
        }
        GameObject.Find("TurnHandler").GetComponent<TurnHandler>().AddEffect(caster, Effect.TrueStrike, 3);
    }

    public override void ActionEffect(Unit caster, Unit enemy)
    {
        throw new System.NotImplementedException();
    }
}
