using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backstab : Action
{
    void Start()
    {
        actionName = "Backstab";
        manaCost = 0;
        priority = 0;
        type = DTypes.Piercing;
        originalType = type;
        basePotency = 20;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        int crit = 0;
        if (caster.GetSpeed() > enemy.GetSpeed()) crit = Random.Range(0, 2) == 1 ? 2 : 1;
        else
        {
            crit = Random.Range(0, 20) == 20 ? 2 : 1;
        }
        float vulnerability = CheckResistance(type, enemy);

        float damage = (caster.GetStrength() / enemy.GetPhysDefence()) * basePotency * vulnerability * crit * Random.Range(0.85f, 1);

        enemy.Damage(damage, type, th);
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.Attack },
            potency = basePotency,
            effect = Effect.Null
        };
    }
}
