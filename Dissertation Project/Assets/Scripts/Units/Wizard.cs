using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Unit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitName = "Wizard";
        maxHealth = 140;
        health = maxHealth;
        maxMana = 100;
        mana = maxMana;
        speed = 35;
        physDefence = 20;
        magicDefence = 15;
        strength = 10;
        magicStrength = 45;
        immunities = new HashSet<DTypes>();
        originalImmunities = new HashSet<DTypes>();
        resistances = new HashSet<DTypes>();
        originalResistances = new HashSet<DTypes>();
        vulnerabilities = new HashSet<DTypes>();
        originalVulnerabilities = new HashSet<DTypes>();
    }
}
