using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Unit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        maxHealth = 100;
        health = maxHealth;
        maxMana = 20;
        mana = maxMana;
        speed = 30;
        physDefence = 40;
        magicDefence = 25;
        strength = 30;
        magicStrength = 10;
        immunities = new HashSet<DTypes>();
        resistances = new HashSet<DTypes>();
        vulnerabilities = new HashSet<DTypes>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
