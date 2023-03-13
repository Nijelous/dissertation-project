using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Lightning Strike";
        manaCost = 30;
        priority = 0;
        type = DTypes.Lightning;
        originalType = type;
        basePotency = 50;
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
