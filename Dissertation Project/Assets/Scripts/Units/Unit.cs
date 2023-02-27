using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected float maxHealth;
    protected float health;
    protected float maxMana;
    protected float mana;
    protected float speed;
    protected float physDefence;
    protected float magicDefence;
    protected float strength;
    protected float magicStrength;
    protected HashSet<DTypes> immunities;
    protected HashSet<DTypes> resistances;
    protected HashSet<DTypes> vulnerabilities;
    protected HashSet<Action> physicalActions;
    protected HashSet<Action> magicalActions;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        physicalActions = new HashSet<Action>();
        magicalActions = new HashSet<Action>();
        foreach (Action c in gameObject.GetComponents(typeof(Action)))
        {
            if(c.GetDamageType() == DTypes.Slashing || c.GetDamageType() == DTypes.Piercing || c.GetDamageType() == DTypes.Bludgeoning)
            {
                physicalActions.Add(c);
            }
            else
            {
                magicalActions.Add(c);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float damage)
    {
        health -= damage;
        if(health < 0) health = 0;
    }

    public float GetHealth() { return health; }

    public void AddHealth(float f)
    {
        if (f + health > maxHealth) health = maxHealth;
        else health += f;
    }

    public float GetMaxHealth() { return maxHealth; }

    public void SetMaxHealth(float f) { maxHealth = f; }

    public float GetMana() { return mana; }

    public void AddMana(float f)
    {
        if (f + mana > maxMana) mana = maxMana;
        else mana += f;
    }

    public float GetMaxMana() { return maxMana; }

    public void SetMaxMana(float f) { maxMana = f; }

    public float GetSpeed() { return speed; }

    public void SetSpeed(float f) { speed = f; }

    public float GetPhysDefence() { return physDefence; }

    public void SetPhysDefence(float f) { physDefence = f; }

    public float GetMagicDefence() { return magicDefence; }

    public void SetMagicDefence(float f) { magicDefence = f; }

    public float GetStrength() { return strength; }

    public void AddStrength(float f) { strength += f; }

    public void SetStrength(float f) { strength = f; }

    public float GetMagicStrength() { return magicStrength; }

    public void SetMagicStrength(float f) { magicStrength = f; }

    public HashSet<DTypes> GetImmunities() { return immunities; }

    public void AddImmunity(DTypes type) { immunities.Add(type); }

    public void RemoveImmunity(DTypes type) { immunities.Remove(type); }

    public HashSet<DTypes> GetResistances() { return resistances; }

    public void AddResistance(DTypes type) { resistances.Add(type); }

    public void RemoveResistance(DTypes type) { resistances.Remove(type); }

    public HashSet<DTypes> GetVulnerabilities() { return vulnerabilities; }

    public void AddVulnerability(DTypes type) { vulnerabilities.Add(type); }

    public void RemoveVulnerability(DTypes type) { vulnerabilities.Remove(type); }

    public HashSet<Action> GetPhysicalActions() { return physicalActions; }

    public void AddPhysicalAction(Action action) { if(action.GetManaCost() == 0) physicalActions.Add(action); }

    public void RemovePhysicalAction(Action action) { if (action.GetManaCost() == 0) physicalActions.Remove(action); }

    public HashSet<Action> GetMagicalActions() { return magicalActions; }

    public void AddMagicalAction(Action action) { if (action.GetManaCost() > 0) magicalActions.Add(action); }

    public void RemoveMagicalAction(Action action) { if (action.GetManaCost() > 0) magicalActions.Remove(action); }


}
