using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondWind : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Second Wind";
        manaCost = 20;
        priority = 0;
        type = DTypes.Holy;
        originalType = type;
        basePotency = 25;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Act(Unit caster, Unit enemy)
    {
        caster.AddHealth(basePotency + (caster.GetMagicStrength() / 10));
    }

    public override void ActionEffect(Unit caster, Unit enemy)
    {
        throw new System.NotImplementedException();
    }
}
