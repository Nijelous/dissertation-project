using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Unit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitName = "Fighter";
        maxHealth = 250;
        health = maxHealth;
        maxMana = 20;
        mana = maxMana;
        speed = 30;
        physDefence = 40;
        magicDefence = 30;
        strength = 15;
        magicStrength = 10;
        immunities = new HashSet<DTypes>();
        originalImmunities = new HashSet<DTypes>();
        resistances = new HashSet<DTypes>();
        originalResistances = new HashSet<DTypes>();
        vulnerabilities = new HashSet<DTypes>();
        originalVulnerabilities = new HashSet<DTypes>();
    }
}
