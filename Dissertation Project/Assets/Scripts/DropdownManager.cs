using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DropdownManager : MonoBehaviour
{
    private Dropdown[] dropdowns;
    private Button showSelection;
    [SerializeField]
    private Unit player1;
    [SerializeField]
    private Unit player2;
    private GameObject[] actionWeights;
    // Start is called before the first frame update
    void Awake()
    {
        dropdowns = new Dropdown[transform.childCount - 2];
        for(int i = 0; i < dropdowns.Length; i++)
        {
            dropdowns[i] = transform.GetChild(i).GetComponent<Dropdown>();
        }
        showSelection = transform.GetChild(transform.childCount - 2).GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (actionWeights == null)
        {
            actionWeights = GameObject.FindGameObjectsWithTag("Weight");
            for (int i = 0; i < 7; i++)
            {
                dropdowns[1].GetComponent<Dropdown>().options[i+1].text = i < 3 ? player1.GetPhysicalActions().ToArray()[i].GetActionName() :
                    player1.GetMagicalActions().ToArray()[i - 3].GetActionName();
            }
        }
    }

    public void PlayerSelection()
    {
        if (dropdowns[0].value == 1)
        {
            for (int i = 0; i < 7; i++)
            {
                dropdowns[1].GetComponent<Dropdown>().options[i + 1].text = i < 3 ? player1.GetPhysicalActions().ToArray()[i].GetActionName() :
                    player1.GetMagicalActions().ToArray()[i - 3].GetActionName();
            }
            for (int i = 0; i < actionWeights.Length; i++)
            {
                actionWeights[i].GetComponent<Text>().fontSize = i < actionWeights.Length / 2 ? 10 : 11;
            }
        }
        if(dropdowns[0].value == 2)
        {
            for (int i = 0; i < 7; i++)
            {
                dropdowns[1].GetComponent<Dropdown>().options[i + 1].text = i < 3 ? player2.GetPhysicalActions().ToArray()[i].GetActionName() :
                    player2.GetMagicalActions().ToArray()[i - 3].GetActionName();
            }
            for (int i = 0; i < actionWeights.Length; i++)
            {
                actionWeights[i].GetComponent<Text>().fontSize = i >= actionWeights.Length / 2 ? 10 : 11;
            }
        }
    }

    public void CanFilter()
    {
        bool canFilter = true;
        for(int i = 0; i < dropdowns.Length; i++) 
        {
            if (dropdowns[i].value == 0)
            {
                canFilter = false;
                break;
            }
        }
        showSelection.interactable = canFilter;
    }

    public void Filter()
    {
        int playerSelection = dropdowns[0].value-1;
        int weightStart;
        if (dropdowns[1].value == 8) weightStart = 0;
        else
        {
            weightStart = 1;
            for(int i = 1; i < dropdowns.Length; i++)
            {
                int valToAdd = dropdowns[i].value-1;
                for(int j = i + 1; j < dropdowns.Length; j++)
                {
                    valToAdd *= dropdowns[j].options.Count - 1;
                }
                weightStart += valToAdd;
            }
            weightStart *= 7;
            weightStart += playerSelection * actionWeights.Length / 2;
        }
        for(int i = 0; i < actionWeights.Length; i++)
        {
            if(i < weightStart || i > weightStart + 6)
            {
                actionWeights[i].GetComponent<Text>().fontSize = 11;
            }
            else
            {
                actionWeights[i].GetComponent<Text>().fontSize = 10;
            }
        }
    }

    public void ResetFilters()
    {
        for(int i = 0; i < actionWeights.Length; i++)
        {
            actionWeights[i].GetComponent<Text>().fontSize = i < actionWeights.Length/2 ? 10 : 11;
        }
        for(int i = 0; i < dropdowns.Length; i++)
        {
            dropdowns[i].value = 0;
        }
    }
}
