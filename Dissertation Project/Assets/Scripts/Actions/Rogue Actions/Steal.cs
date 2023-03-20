using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steal : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Steal";
        manaCost = 0;
        priority = 0;
        type = DTypes.Slashing;
        originalType = type;
        basePotency = 20;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Act(Unit caster, Unit enemy, TurnHandler th)
    {
        float manaGain;
        if (enemy.GetMana() < basePotency)
        {
            if (caster.GetMaxMana() - caster.GetMana() < enemy.GetMana())
            {
                manaGain = caster.GetMaxMana() - caster.GetMana();
            }
            else
            {
                manaGain = enemy.GetMana();
            }
        }
        else
        {
            if (caster.GetMaxMana() - caster.GetMana() < basePotency)
            {
                manaGain = caster.GetMaxMana() - caster.GetMana();
            }
            else
            {
                manaGain = basePotency;
            }
        }
        caster.AddMana(manaGain);
        enemy.AddMana(-manaGain);
    }

    public override ActionDetails ActionEffect()
    {
        return new ActionDetails()
        {
            types = new ActionTypes[] { ActionTypes.ManaRecovery },
            potency = basePotency,
            effect = Effect.Null
        };
    }
}
