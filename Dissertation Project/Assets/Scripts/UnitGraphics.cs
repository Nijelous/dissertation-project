using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitGraphics : MonoBehaviour
{
    [SerializeField]
    private Unit unit;

    [SerializeField]
    private GameObject bars;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float healthPercentage = unit.GetHealth()/unit.GetMaxHealth();
        float manaPercentage = unit.GetMana()/unit.GetMaxMana();
        bars.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(healthPercentage * 200, 30);
        bars.transform.GetChild(1).GetComponent<Text>().text = Mathf.Ceil(unit.GetHealth()) + "/" + unit.GetMaxHealth();
        bars.transform.GetChild(2).GetComponent<RectTransform>().sizeDelta = new Vector2(manaPercentage * 200, 30);
        bars.transform.GetChild(3).GetComponent<Text>().text = Mathf.Ceil(unit.GetMana()) + "/" + unit.GetMaxMana();
        bars.transform.GetChild(4).GetComponent<Text>().text = unit.name;
    }
}
