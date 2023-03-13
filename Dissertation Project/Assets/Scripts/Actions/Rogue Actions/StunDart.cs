using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunDart : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Stun Dart";
        manaCost = 30;
        priority = 1;
        type = DTypes.Lightning;
        originalType = type;
        basePotency = 20;
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

        if(th.CheckForEffect(enemy, Effect.StunResist) == -1)
        {
            th.AddEffect(enemy, Effect.Stunned, 1);
        }
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.Attack, ActionTypes.Status },
            potency = basePotency,
            effect = Effect.Stunned
        };
    }
}
