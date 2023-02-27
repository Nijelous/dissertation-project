using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Action
{
    // Start is called before the first frame update
    void Start()
    {
        actionName = "Slash";
        manaCost = 0;
        priority = 0;
        type = DTypes.Slashing;
        originalType = type;
        basePotency = 50;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Act(Unit caster, Unit enemy)
    {
        int crit = Random.Range(0, 20) == 20 ? 2 : 1;

        float vulnerability = CheckResistance(type, enemy);

        float damage = (caster.GetStrength() / enemy.GetPhysDefence()) * basePotency * vulnerability * crit * Random.Range(0.85f, 1);

        enemy.Damage(damage);
    }

    public override void ActionEffect(Unit caster, Unit enemy)
    {
        throw new System.NotImplementedException();
    }
}
