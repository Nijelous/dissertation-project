using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Unit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitName = "Rogue";
        maxHealth = 80;
        health = maxHealth;
        maxMana = 50;
        mana = maxMana;
        speed = 40;
        physDefence = 15;
        magicDefence = 15;
        strength = 40;
        magicStrength = 15;
        immunities = new HashSet<DTypes>();
        originalImmunities = new HashSet<DTypes>();
        resistances = new HashSet<DTypes>();
        originalResistances = new HashSet<DTypes>();
        vulnerabilities = new HashSet<DTypes>();
        originalVulnerabilities = new HashSet<DTypes>();
    }
}
