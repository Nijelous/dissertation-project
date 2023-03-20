using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThousandCuts : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Thousand Cuts";
        manaCost = 0;
        priority = 0;
        type = DTypes.Piercing;
        originalType = type;
        basePotency = 7;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        int hits = Random.Range(2, 6);
        for (int i = 0; i < hits; i++)
        {
            int crit = Random.Range(0, 20) == 20 ? 2 : 1;

            float vulnerability = CheckResistance(type, enemy);

            float damage = (caster.GetStrength() / enemy.GetPhysDefence()) * basePotency * vulnerability * crit * Random.Range(0.85f, 1);

            enemy.Damage(damage, type, th);
        }
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.Attack },
            potency = basePotency * 2.5f,
            effect = Effect.Null
        };
    }
}
