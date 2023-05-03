using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourCrush : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Armour Crush";
        manaCost = 0;
        priority = 0;
        type = DTypes.Bludgeoning;
        originalType = type;
        basePotency = 35;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        int crit = Random.Range(0, 20) == 20 ? 2 : 1;

        float vulnerability = CheckResistance(type, enemy);

        float damage = (caster.GetStrength() / enemy.GetPhysDefence()) * basePotency * vulnerability * crit * Random.Range(0.85f, 1);

        enemy.Damage(damage, type, th);

        if(!th.AddEffect(enemy, Effect.ArmourDown, 4))
        {
            enemy.AddPhysDefence(-enemy.GetPhysDefence()/2);
        }
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.Attack, ActionTypes.Status },
            potency = basePotency,
            effect = Effect.ArmourDown
        };
    }
}
