using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Unit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        maxHealth = 60;
        health = maxHealth;
        maxMana = 100;
        mana = maxMana;
        speed = 35;
        physDefence = 15;
        magicDefence = 15;
        strength = 10;
        magicStrength = 50;
        immunities = new HashSet<DTypes>();
        originalImmunities = new HashSet<DTypes>();
        resistances = new HashSet<DTypes>();
        originalResistances = new HashSet<DTypes>();
        vulnerabilities = new HashSet<DTypes>();
        originalVulnerabilities = new HashSet<DTypes>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
