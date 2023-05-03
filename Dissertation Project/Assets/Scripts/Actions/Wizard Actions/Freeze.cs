using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Freeze";
        manaCost = 80;
        priority = 0;
        type = DTypes.Ice;
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

        float damage = (caster.GetMagicStrength() / enemy.GetMagicDefence()) * basePotency * vulnerability * crit * Random.Range(0.85f, 1);

        enemy.Damage(damage, type, th);

        if (th.CheckForEffect(enemy, Effect.StunResist) == -1)
        {
            int duration = Random.Range(2, 4);
            th.AddEffect(enemy, Effect.Stunned, duration);
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
