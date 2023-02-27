using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ControlType
{
    Human,
    Random
}
public class Controller : MonoBehaviour
{
    [SerializeField]
    private Unit player1;

    [SerializeField] 
    private Unit player2;

    [SerializeField]
    private ControlType ct1;

    [SerializeField] 
    private ControlType ct2;

    [SerializeField]
    private GameObject playerSelect;

    [SerializeField]
    private Text combatText;

    [SerializeField]
    private TurnHandler th;

    [SerializeField]
    private bool quickCombat;

    [SerializeField]
    private Text roundNumber;

    private float waitTime = 3;

    private bool isPlayer1Turn = true;

    private Action player1Action;

    private bool roundRunning = true;

    private bool hasWinner = false;

    private int round = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (quickCombat) waitTime = 0;
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        if (ct1 != ControlType.Human && ct2 == ControlType.Human)
        {
            isPlayer1Turn = false;
            SetButtons(player2);
        }
        else if (ct1 == ControlType.Human || ct2 == ControlType.Human)
        {
            SetButtons(player1);
        }
        else
        {
            playerSelect.SetActive(false);
        }
        roundRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(ct1 != ControlType.Human && ct2 != ControlType.Human && !roundRunning && !hasWinner)
        {
            // AI vs AI
            roundRunning = true;
            StartCoroutine(TakeTurn(GetTurnAction(player1, ct1), GetTurnAction(player2, ct2)));
        }
    }

    public void ActivateButtons(GameObject buttons)
    {
        buttons.SetActive(true);
    }

    private void SetButtons(Unit unit)
    {
        for(int i = 0; i < 3; i++)
        {
            playerSelect.transform.GetChild(2).GetChild(i).GetComponentInChildren<Text>().text = unit.GetPhysicalActions().ToArray()[i].GetActionName();
        }
        for(int i = 0; i < 4; i++)
        {
            playerSelect.transform.GetChild(4).GetChild(i).GetComponentInChildren<Text>().text = unit.GetMagicalActions().ToArray()[i].GetActionName();
            if (unit.GetMagicalActions().ToArray()[i].GetManaCost() > unit.GetMana())
            {
                playerSelect.transform.GetChild(4).GetChild(i).GetComponent<Button>().interactable = false;
            }
            else
            {
                playerSelect.transform.GetChild(4).GetChild(i).GetComponent<Button>().interactable = true;
            }
        }
    }
    public void PlayerAction(int action)
    {
        Action a = null;
        if(isPlayer1Turn) {
            switch (action)
            {
                case 0: a = player1.GetPhysicalActions().ToArray()[0]; break;
                case 1: a = player1.GetPhysicalActions().ToArray()[1]; break;
                case 2: a = player1.GetPhysicalActions().ToArray()[2]; break;
                case 3: a = player1.GetMagicalActions().ToArray()[0]; break;
                case 4: a = player1.GetMagicalActions().ToArray()[1]; break;
                case 5: a = player1.GetMagicalActions().ToArray()[2]; break;
                case 6: a = player1.GetMagicalActions().ToArray()[3]; break;
            }
        }
        else
        {
            switch (action)
            {
                case 0: a = player2.GetPhysicalActions().ToArray()[0]; break;
                case 1: a = player2.GetPhysicalActions().ToArray()[1]; break;
                case 2: a = player2.GetPhysicalActions().ToArray()[2]; break;
                case 3: a = player2.GetMagicalActions().ToArray()[0]; break;
                case 4: a = player2.GetMagicalActions().ToArray()[1]; break;
                case 5: a = player2.GetMagicalActions().ToArray()[2]; break;
                case 6: a = player2.GetMagicalActions().ToArray()[3]; break;
            }
        }
        if (ct1 == ControlType.Human && ct2 == ControlType.Human)
        {
            if (isPlayer1Turn)
            {
                player1Action = a;
                isPlayer1Turn = false;
                SetButtons(player2);
            }
            else
            {
                StartCoroutine(TakeTurn(player1Action, a));
                SetButtons(player1);
            }
        }
        else
        {
            if (ct1 == ControlType.Human)
            {
                StartCoroutine(TakeTurn(a, GetTurnAction(player2, ct2)));
            }
            else if (ct2 == ControlType.Human)
            {
                StartCoroutine(TakeTurn(GetTurnAction(player1, ct1), a));
            }
        }
    }
    private Action GetTurnAction(Unit unit, ControlType ct)
    {
        switch (ct)
        {
            case ControlType.Random:
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    random = Random.Range(0, 3);
                    return unit.GetPhysicalActions().ToArray()[random];
                }
                else
                {
                    random = Random.Range(0, 4);
                    if (unit.GetMagicalActions().ToArray()[random].GetManaCost() <= unit.GetMana())
                    {
                        return unit.GetMagicalActions().ToArray()[random];
                    }
                    else
                    {
                        return GetTurnAction(unit, ct);
                    }
                }
            case ControlType.Human:
                return unit.GetPhysicalActions().ToArray()[0];
        }
        return null;
    }

    IEnumerator TakeTurn(Action unit1Action, Action unit2Action)
    {
        Debug.Log(roundNumber.text);
        if (ct1 == ControlType.Human || ct2 == ControlType.Human) playerSelect.SetActive(false);
        combatText.gameObject.SetActive(true);
        bool player1First = true;
        if(unit1Action.GetPriority() > unit2Action.GetPriority())
        {
            player1First = true;
        }
        else if(unit1Action.GetPriority() < unit2Action.GetPriority())
        {
            player1First = false;
        }
        else
        {
            if(player1.GetSpeed() > player2.GetSpeed())
            {
                player1First = true;
            }
            else if(player1.GetSpeed() < player2.GetSpeed())
            {
                player1First = false;
            }
            else
            {
                switch(Random.Range(0, 2)) {
                    case 0:
                        player1First = true;
                        break;
                    case 1:
                        player1First = false;
                        break;
                }
            }
        }
        if (player1First)
        {
            Round(player1, player2, unit1Action);
            yield return new WaitForSeconds(waitTime);
            if (hasWinner) { yield break; }
            Round(player2, player1, unit2Action);
            yield return new WaitForSeconds(waitTime);
            if (hasWinner) { yield break; }
        }
        else
        {
            Round(player2, player1, unit2Action);
            yield return new WaitForSeconds(waitTime);
            if (hasWinner) { yield break; }
            Round(player1, player2, unit1Action);
            yield return new WaitForSeconds(waitTime);
            if (hasWinner) { yield break; }

        }
        th.TickEffects();
        if (ct1 == ControlType.Human || ct2 == ControlType.Human) playerSelect.SetActive(true);
        if (ct1 != ControlType.Human && ct2 == ControlType.Human) SetButtons(player2);
        else SetButtons(player1);
        combatText.gameObject.SetActive(false);
        round++;
        roundNumber.text = "Round " + round;
        roundRunning = false;
    }

    public void Round(Unit caster, Unit enemy, Action action)
    {
        combatText.text = "" + caster.name + " used " + action.GetActionName();
        Debug.Log(caster.name + " used " + action.GetActionName());
        caster.AddMana(-action.GetManaCost());
        action.Act(caster, enemy);
        if(caster.GetHealth() == 0) {
            Win(enemy, caster);
        }
        else if(enemy.GetHealth() == 0)
        {
            Win(caster, enemy);
        }
    }

    public void Win(Unit winner, Unit loser)
    {
        Debug.Log(winner.name + " Wins");
        th.ClearEffects();
        hasWinner = true;
    }
}
