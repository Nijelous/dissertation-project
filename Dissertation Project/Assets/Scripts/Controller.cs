using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum ControlType
{
    Human,
    Random,
    GeneticPair,
    GeneticSoloBasic,
    GeneticSoloAdvanced
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

    [SerializeField]
    private GameObject evolutionGO;

    private Evolution evolution;

    private bool evaluating = true;

    private Candidate candidate1;

    private Candidate candidate2;

    private int[] actionsUsed;

    public void SetController(Unit player1, Unit player2, ControlType ct1, ControlType ct2, TurnHandler th, bool quickCombat, float waitTime)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.ct1 = ct1;
        this.ct2 = ct2;
        this.th = th;
        this.quickCombat = quickCombat;
        if (quickCombat) this.waitTime = waitTime;
    }

    // Start is called before the first frame update
    void Awake()
    {
        actionsUsed = new int[2];
        if (quickCombat) waitTime = 0;
        if(ct1 == ControlType.GeneticPair)
        {
            evolution = GameObject.Find("Evolution").GetComponent<Evolution>();
            evaluating = false;
        }
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        if (playerSelect)
        {
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
        }
        if (ct1 == ControlType.GeneticSoloBasic || ct1 == ControlType.GeneticSoloAdvanced)
        {
            string path;
            string geneString;
            string complexity = "";
            if (ct1 == ControlType.GeneticSoloBasic) complexity = "Basic";
            else if (ct1 == ControlType.GeneticSoloAdvanced) complexity = "Advanced";
            if (player1.GetUnitName() == player2.GetUnitName())
            {
                path = Application.dataPath + "/Best Candidates/" + player1.GetUnitName() + "-" + player2.GetUnitName() + "-" + complexity + "-1";
            }
            else
            {
                path = Application.dataPath + "/Best Candidates/" + player1.GetUnitName() + "-" + player2.GetUnitName() + "-" + complexity;
            }
            StreamReader sr = new(path);
            geneString = sr.ReadToEnd();
            sr.Close();
            int[] genes = new int[geneString.Count(x => x == ' ')];
            int j = 0;
            for(int i = 0; i < genes.Length; i++)
            {
                bool flag = false;
                int start = j;
                while (!flag)
                {
                    if (geneString[j] == ' ')
                    {
                        flag = true;
                        genes[i] = int.Parse(geneString[start..j]);
                    }
                    j++;
                }
            }
            candidate1 = new Candidate(genes);
        }
        if (ct2 == ControlType.GeneticSoloBasic || ct2 == ControlType.GeneticSoloAdvanced)
        {
            string path;
            string geneString;
            string complexity = "";
            if (ct2 == ControlType.GeneticSoloBasic) complexity = "Basic";
            else if (ct2 == ControlType.GeneticSoloAdvanced) complexity = "Advanced";
            if (player1.GetUnitName() == player2.GetUnitName())
            {
                path = Application.dataPath + "/Best Candidates/" + player2.GetUnitName() + "-" + player1.GetUnitName() + "-" + complexity + "-2";
            }
            else
            {
                path = Application.dataPath + "/Best Candidates/" + player2.GetUnitName() + "-" + player1.GetUnitName() + "-" + complexity;
            }
            StreamReader sr = new(path);
            geneString = sr.ReadToEnd();
            sr.Close();
            Debug.Log(geneString);
            int[] genes = new int[geneString.Count(x => x == ' ')];
            int j = 0;
            for (int i = 0; i < genes.Length; i++)
            {
                bool flag = false;
                int start = j;
                while (!flag)
                {
                    if (geneString[j] == ' ')
                    {
                        flag = true;
                        genes[i] = int.Parse(geneString[start..j]);
                    }
                    j++;
                }
            }
            candidate2 = new Candidate(genes);
            Debug.Log(candidate2.GetWeightsAsString());
            Debug.Log(candidate2.GetTypeWeightsAsString());
        }
        roundRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(ct1 != ControlType.Human && ct2 != ControlType.Human &&  evaluating && !roundRunning && !hasWinner)
        {
            // AI vs AI
            roundRunning = true;
            if (ct1 == ControlType.GeneticPair) TakeTurnAI(GetTurnAction(player1, player2, ct1, 0), GetTurnAction(player2, player1, ct2, 1));
            else StartCoroutine(TakeTurn(GetTurnAction(player1, player2, ct1, 0), GetTurnAction(player2, player1, ct2, 1)));
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
            a = GetActionFromInt(action, player1);
        }
        else
        {
            a = GetActionFromInt(action, player2);
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
                isPlayer1Turn = true;
                SetButtons(player1);
            }
        }
        else
        {
            if (ct1 == ControlType.Human)
            {
                StartCoroutine(TakeTurn(a, GetTurnAction(player2, player1, ct2, 1)));
            }
            else if (ct2 == ControlType.Human)
            {
                StartCoroutine(TakeTurn(GetTurnAction(player1, player2, ct1, 0), a));
            }
        }
    }
    public Action GetActionFromInt(int action, Unit unit)
    {
        if (unit == player1) actionsUsed[0] = action;
        else actionsUsed[1] = action;
        switch (action)
        {
            case 0: return unit.GetPhysicalActions().ToArray()[0];
            case 1: return unit.GetPhysicalActions().ToArray()[1];
            case 2: return unit.GetPhysicalActions().ToArray()[2];
            case 3: return unit.GetMagicalActions().ToArray()[0];
            case 4: return unit.GetMagicalActions().ToArray()[1];
            case 5: return unit.GetMagicalActions().ToArray()[2];
            case 6: return unit.GetMagicalActions().ToArray()[3];
        }
        return null;
    }
    public Action GetTurnAction(Unit unit, Unit enemy, ControlType ct, int playerTurn)
    {
        switch (ct)
        {
            case ControlType.Random:
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    random = Random.Range(0, 3);
                    actionsUsed[playerTurn] = random;
                    return unit.GetPhysicalActions().ToArray()[random];
                }
                else
                {
                    random = Random.Range(0, 4);
                    if (unit.GetMagicalActions().ToArray()[random].GetManaCost() <= unit.GetMana())
                    {
                        actionsUsed[playerTurn] = random + 3;
                        return unit.GetMagicalActions().ToArray()[random];
                    }
                    else
                    {
                        return GetTurnAction(unit, enemy, ct, 0);
                    }
                }
            case ControlType.Human:
                return unit.GetPhysicalActions().ToArray()[0];
            case ControlType.GeneticPair:
                return GetActionFromInt(evolution.GetCandidateAction(int.Parse(gameObject.name[4..]), playerTurn, unit, enemy, th), unit);
            case ControlType.GeneticSoloBasic:
                if(playerTurn == 0) return GetActionFromInt(candidate1.GetCandidateAction(unit, enemy, th), unit);
                else return GetActionFromInt(candidate2.GetCandidateAction(unit, enemy, th), unit);
            case ControlType.GeneticSoloAdvanced:
                if (playerTurn == 0) return GetActionFromInt(candidate1.GetCandidateAction(unit, enemy, th), unit);
                else return GetActionFromInt(candidate2.GetCandidateAction(unit, enemy, th), unit);
        }
        return null;
    }

    public IEnumerator TakeTurn(Action unit1Action, Action unit2Action)
    {
        if (ct1 != ControlType.GeneticPair) Debug.Log(roundNumber.text);
        if (ct1 == ControlType.Human || ct2 == ControlType.Human) playerSelect.SetActive(false);
        if(ct1 != ControlType.GeneticPair) combatText.gameObject.SetActive(true);
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
        th.TickEffects(player1, player2, actionsUsed[0], actionsUsed[1]);
        if (player1.GetHealth() == 0) Win(player2, player1);
        else if (player2.GetHealth() == 0) Win(player1, player2);
        if (playerSelect)
        {
            if (ct1 == ControlType.Human || ct2 == ControlType.Human) playerSelect.SetActive(true);
            if (ct1 != ControlType.Human && ct2 == ControlType.Human) SetButtons(player2);
            else SetButtons(player1);
        }
        if(ct1 != ControlType.GeneticPair) combatText.gameObject.SetActive(false);
        round++;
        if(ct1 != ControlType.GeneticPair) roundNumber.text = "Round " + round;
        roundRunning = false;
    }

    private void TakeTurnAI(Action unit1Action, Action unit2Action)
    {
        if (ct1 != ControlType.GeneticPair) Debug.Log("Round: " + round);
        if (ct1 == ControlType.Human || ct2 == ControlType.Human) playerSelect.SetActive(false);
        if (ct1 != ControlType.GeneticPair) combatText.gameObject.SetActive(true);
        bool player1First = true;
        if (unit1Action.GetPriority() > unit2Action.GetPriority())
        {
            player1First = true;
        }
        else if (unit1Action.GetPriority() < unit2Action.GetPriority())
        {
            player1First = false;
        }
        else
        {
            if (player1.GetSpeed() > player2.GetSpeed())
            {
                player1First = true;
            }
            else if (player1.GetSpeed() < player2.GetSpeed())
            {
                player1First = false;
            }
            else
            {
                switch (Random.Range(0, 2))
                {
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
            if (hasWinner) { return; }
            Round(player2, player1, unit2Action);
            if (hasWinner) { return; }
        }
        else
        {
            Round(player2, player1, unit2Action);
            if (hasWinner) { return; }
            Round(player1, player2, unit1Action);
            if (hasWinner) { return; ; }

        }
        th.TickEffects(player1, player2, actionsUsed[0], actionsUsed[1]);
        if (player1.GetHealth() == 0) Win(player2, player1);
        else if (player2.GetHealth() == 0) Win(player1, player2);
        if (playerSelect)
        {
            if (ct1 == ControlType.Human || ct2 == ControlType.Human) playerSelect.SetActive(true);
            if (ct1 != ControlType.Human && ct2 == ControlType.Human) SetButtons(player2);
            else SetButtons(player1);
        }
        if (ct1 != ControlType.GeneticPair) combatText.gameObject.SetActive(false);
        round++;
        if (ct1 != ControlType.GeneticPair) roundNumber.text = "Round " + round;
        roundRunning = false;
    }

    public void Round(Unit caster, Unit enemy, Action action)
    {
        if (caster.GetMana() < action.GetManaCost())
        {
            if (ct1 != ControlType.GeneticPair) combatText.text = "" + caster.name + " no longer has the mana to use " + action.GetActionName();
            if (ct1 != ControlType.GeneticPair) Debug.Log(caster.name + " no longer has the mana to use " + action.GetActionName());
        }
        else if (th.CheckForEffect(caster, Effect.Stunned) == -1)
        {
            if (ct1 != ControlType.GeneticPair) combatText.text = "" + caster.name + " used " + action.GetActionName();
            if (ct1 != ControlType.GeneticPair) Debug.Log(caster.name + " used " + action.GetActionName());
            caster.AddMana(-action.GetManaCost());
            action.Act(caster, enemy, th);
            if (caster.GetHealth() == 0)
            {
                Win(enemy, caster);
            }
            else if (enemy.GetHealth() == 0)
            {
                Win(caster, enemy);
            }
        }
        else
        {
            if (ct1 != ControlType.GeneticPair) combatText.text = "" + caster.name + " is Stunned, and cannot move";
            if (ct1 != ControlType.GeneticPair) Debug.Log(caster.name + " is Stunned, and cannot move");
        }
    }

    public void Win(Unit winner, Unit loser)
    {
        if (ct1 != ControlType.GeneticPair) Debug.Log(winner.name + " Wins");
        th.ClearEffects();
        hasWinner = true;
        evaluating = false;
    }

    public void Evaluate()
    {
        evaluating = true;
    }

    public bool FinishedEvaluating(int maxRounds)
    {
        return !evaluating || round > maxRounds;
    }

    public int GetRound()
    {
        return round;
    }

    public void ResetBattle()
    {
        round = 1;
        hasWinner = false;
        player1.AddHealth(player1.GetMaxHealth());
        player1.AddMana(player1.GetMaxMana());
        player2.AddHealth(player2.GetMaxHealth());
        player2.AddMana(player2.GetMaxMana());
        actionsUsed = new int[2];
        roundRunning = false;
    }
}
