using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action : MonoBehaviour
{
    protected string actionName;
    protected float manaCost;
    protected int priority;
    protected DTypes type;
    protected DTypes originalType;
    protected float basePotency;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Act(Unit caster, Unit enemy);

    public abstract void ActionEffect(Unit caster, Unit enemy);

    public float GetManaCost() { return manaCost; }

    public void SetManaCost(float cost) { manaCost = cost; }

    public int GetPriority() { return priority; }

    public void SetPriority(int p) { priority = p; }

    public DTypes GetDamageType() { return type; }

    public void SetDamageType(DTypes t) { type = t; }

    public DTypes GetOriginalDamageType() { return originalType; }

    public void ResetDamageType() { type = originalType; }

    public float GetBasePotency() { return basePotency; }

    public void SetBasePotency(float p) { basePotency = p; }

    public float CheckResistance(DTypes type, Unit unit)
    {
        if (unit.GetImmunities().Contains(type)) return 0;

        else if (unit.GetResistances().Contains(type)) return 0.5f;

        else if (unit.GetVulnerabilities().Contains(type)) return 2;

        return 1;
    }

    public string GetActionName() { return actionName; }
}
