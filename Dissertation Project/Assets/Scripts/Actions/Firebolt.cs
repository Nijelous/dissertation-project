using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Firebolt";
        manaCost = 10;
        priority = 0;
        type = DTypes.Fire;
        originalType = type;
        basePotency = 30;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Act(Unit caster, Unit enemy)
    {
        int crit = Random.Range(0, 20) == 20 ? 2 : 1;

        float vulnerability = CheckResistance(type, enemy);

        float damage = (caster.GetMagicStrength() / enemy.GetMagicDefence()) * basePotency * vulnerability * crit * Random.Range(0.85f, 1);

        enemy.Damage(damage);

    }

    public override void ActionEffect(Unit caster, Unit enemy)
    {
        throw new System.NotImplementedException();
    }
}
