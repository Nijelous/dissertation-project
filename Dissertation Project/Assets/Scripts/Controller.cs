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
    GeneticSoloAdvanced,
    GeneticSoloAdvancedTrainingPartner,
    GeneticSoloAdvancedHeuristic,
    Reinforcement,
    ReinforcementVsAll,
    ReinforcementLearning
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
    [Range(0, 3)]
    private int turnsRemembered1;

    [SerializeField]
    [Range(0, 3)]
    private int turnsRemembered2;

    [SerializeField]
    [Range(0, 1)]
    private float healthBarCutoff;

    [SerializeField]
    [Range(0, 1)]
    private float manaBarCutoff;

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

    private Evolution evolution;

    private Training training;

    private bool evaluating = true;

    private bool logging = false;

    private Candidate candidate1;

    private Candidate candidate2;

    private QActor actor1;

    private QActor actor2;

    private int[] actionsUsed;

    private int[] wins;

    private int[] results;

    private int battles;

    [SerializeField]
    [Range(1, 3)]
    private int set;

    private int count1;

    private int ciel1;

    private int count2;

    private int ciel2;

    private string combatLog;


    public void SetController(Unit player1, Unit player2, ControlType ct1, ControlType ct2, TurnHandler th, bool quickCombat, float waitTime, int[] turnsRemembered, float healthBarCutoff, float manaBarCutoff)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.ct1 = ct1;
        this.ct2 = ct2;
        this.th = th;
        this.quickCombat = quickCombat;
        if (quickCombat) this.waitTime = waitTime;
        turnsRemembered1 = turnsRemembered[0];
        turnsRemembered2 = turnsRemembered[1];
        this.healthBarCutoff = healthBarCutoff;
        this.manaBarCutoff = manaBarCutoff;
    }

    public void SetController(Unit player1, Unit player2, ControlType ct1, ControlType ct2, TurnHandler th, bool quickCombat, float waitTime, int[] turnsRemembered, float healthBarCutoff, float manaBarCutoff, Training training)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.ct1 = ct1;
        this.ct2 = ct2;
        this.th = th;
        this.quickCombat = quickCombat;
        if (quickCombat) this.waitTime = waitTime;
        turnsRemembered1 = turnsRemembered[0];
        turnsRemembered2 = turnsRemembered[1];
        this.healthBarCutoff = healthBarCutoff;
        this.manaBarCutoff = manaBarCutoff;
        this.training = training;
    }

    // Start is called before the first frame update
    void Awake()
    {
        wins = new int[2];
        results = new int[100];
        battles = 0;
        count1 = 0;
        count2 = 0;
        actionsUsed = new int[2];
        string path = Application.dataPath + "/Graphs/Results.csv";
        File.WriteAllText(path, "");
        if (quickCombat) waitTime = 0.01f;
        if(ct1 == ControlType.GeneticPair)
        {
            evolution = GameObject.Find("Evolution").GetComponent<Evolution>();
            evaluating = false;
        }
        if(ct1 != ControlType.Human && ct2 != ControlType.Human)
        {
            switch (ct1)
            {
                case ControlType.Random:
                    player1.gameObject.name = "Random";
                    ciel1 = 1;
                    break;
                case ControlType.GeneticSoloBasic:
                    player1.gameObject.name = "Basic";
                    ciel1 = 3;
                    break;
                case ControlType.GeneticSoloAdvanced:
                    player1.gameObject.name = "Advanced " + turnsRemembered1;
                    ciel1 = 3;
                    break;
                case ControlType.GeneticSoloAdvancedTrainingPartner:
                    player1.gameObject.name = "Advanced+ " + turnsRemembered1;
                    ciel1 = 1;
                    break;
                case ControlType.GeneticSoloAdvancedHeuristic:
                    player1.gameObject.name = "Advanced H " + turnsRemembered1;
                    ciel1 = 3;
                    break;
                case ControlType.Reinforcement:
                    player1.gameObject.name = "Q " + turnsRemembered1;
                    ciel1 = 1;
                    break;
                case ControlType.ReinforcementVsAll:
                    player1.gameObject.name = "QVsAll " + turnsRemembered1;
                    ciel1 = 1;
                    break;
            }
            switch (ct2)
            {
                case ControlType.Random:
                    player2.gameObject.name = "Random";
                    ciel2 = 1;
                    break;
                case ControlType.GeneticSoloBasic:
                    player2.gameObject.name = "Basic";
                    ciel2 = 3;
                    break;
                case ControlType.GeneticSoloAdvanced:
                    player2.gameObject.name = "Advanced " + turnsRemembered2;
                    ciel2 = 3;
                    break;
                case ControlType.GeneticSoloAdvancedTrainingPartner:
                    player2.gameObject.name = "Advanced+ " + turnsRemembered2;
                    ciel2 = 1;
                    break;
                case ControlType.GeneticSoloAdvancedHeuristic:
                    player2.gameObject.name = "Advanced H " + turnsRemembered2;
                    ciel2 = 3;
                    break;
                case ControlType.Reinforcement:
                    player2.gameObject.name = "Q " + turnsRemembered2;
                    ciel2 = 1;
                    break;
                case ControlType.ReinforcementVsAll:
                    player2.gameObject.name = "QVsAll " + turnsRemembered2;
                    ciel2 = 1;
                    break;
            }
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
        if (ct1 == ControlType.GeneticSoloBasic ||  ct1 == ControlType.GeneticSoloAdvanced || ct1 == ControlType.GeneticSoloAdvancedTrainingPartner || ct1 == ControlType.GeneticSoloAdvancedHeuristic)
        {
            
            string geneString;
            string complexity = "";
            if (ct1 == ControlType.GeneticSoloBasic) complexity = "Basic";
            else if (ct1 == ControlType.GeneticSoloAdvanced) complexity = "Advanced";
            else if (ct1 == ControlType.GeneticSoloAdvancedTrainingPartner) complexity = "Advanced+";
            else if (ct1 == ControlType.GeneticSoloAdvancedHeuristic) complexity = "AdvancedHeuristic";
            string path = Application.dataPath + "/Best Candidates/" + player1.GetUnitName() + "-" + player2.GetUnitName() + "-" + complexity + "-" + set;
            if (ct1 == ControlType.GeneticSoloAdvanced || ct1 == ControlType.GeneticSoloAdvancedTrainingPartner || ct1 == ControlType.GeneticSoloAdvancedHeuristic) path += "-" + turnsRemembered1;
            Debug.Log(path);
            Debug.Log("Player 1: " + complexity + (ct1 != ControlType.GeneticSoloBasic ? (" " + turnsRemembered1) : "") + " Generation 1");
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
            candidate1 = new Candidate(genes, turnsRemembered1, healthBarCutoff, manaBarCutoff);
        }
        else if(ct1 == ControlType.Reinforcement || ct1 == ControlType.ReinforcementVsAll)
        {
            string tableString;
            string p2 = "";
            if (ct1 == ControlType.Reinforcement) p2 = player1.GetUnitName();
            if (ct1 == ControlType.ReinforcementVsAll) p2 = "All";
            string path = Application.dataPath + "/Best Actors/" + player1.GetUnitName() + "-" + p2 + "-" + turnsRemembered1;
            Debug.Log(path);
            Debug.Log("Player 1: Reinforcement Vs " + p2 + " Turns Remembered " + turnsRemembered1);
            StreamReader sr = new(path);
            tableString = sr.ReadToEnd();
            sr.Close();
            int[] table = new int[tableString.Count(x => x == ' ')];
            int j = 0;
            for (int i = 0; i < table.Length; i++)
            {
                bool flag = false;
                int start = j;
                while (!flag)
                {
                    if (tableString[j] == ' ')
                    {
                        flag = true;
                        table[i] = int.Parse(tableString[start..j]);
                    }
                    j++;
                }
            }
            actor1 = new QActor(table, turnsRemembered1, healthBarCutoff, manaBarCutoff);
        }
        if (ct2 == ControlType.GeneticSoloBasic || ct2 == ControlType.GeneticSoloAdvanced || ct2 == ControlType.GeneticSoloAdvancedTrainingPartner || ct2 == ControlType.GeneticSoloAdvancedHeuristic)
        {
            string geneString;
            string complexity = "";
            if (ct2 == ControlType.GeneticSoloBasic) complexity = "Basic";
            else if (ct2 == ControlType.GeneticSoloAdvanced) complexity = "Advanced";
            else if (ct2 == ControlType.GeneticSoloAdvancedTrainingPartner) complexity = "Advanced+";
            else if (ct2 == ControlType.GeneticSoloAdvancedHeuristic) complexity = "AdvancedHeuristic";
            string path = Application.dataPath + "/Best Candidates/" + player2.GetUnitName() + "-" + player1.GetUnitName() + "-" + complexity + "-" + set;
            if (ct2 == ControlType.GeneticSoloAdvanced || ct2 == ControlType.GeneticSoloAdvancedTrainingPartner || ct2 == ControlType.GeneticSoloAdvancedHeuristic) path += "-" + turnsRemembered2;
            Debug.Log("Player 2: " + complexity + (ct2 != ControlType.GeneticSoloBasic ? (" " + turnsRemembered2) : "") + " Generation 1");
            StreamReader sr = new(path);
            geneString = sr.ReadToEnd();
            sr.Close();
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
            candidate2 = new Candidate(genes, turnsRemembered2, healthBarCutoff, manaBarCutoff);
        }
        else if (ct2 == ControlType.Reinforcement || ct2 == ControlType.ReinforcementVsAll)
        {
            string tableString;
            string p2 = "";
            if (ct2 == ControlType.Reinforcement) p2 = player2.GetUnitName();
            if (ct2 == ControlType.ReinforcementVsAll) p2 = "All";
            string path = Application.dataPath + "/Best Actors/" + player2.GetUnitName() + "-" + p2 + "-" + turnsRemembered2;
            Debug.Log(path);
            Debug.Log("Player 2: Reinforcement Vs " + p2 + " Turns Remembered " + turnsRemembered2);
            StreamReader sr = new(path);
            tableString = sr.ReadToEnd();
            sr.Close();
            int[] table = new int[tableString.Count(x => x == ' ')];
            int j = 0;
            for (int i = 0; i < table.Length; i++)
            {
                bool flag = false;
                int start = j;
                while (!flag)
                {
                    if (tableString[j] == ' ')
                    {
                        flag = true;
                        table[i] = int.Parse(tableString[start..j]);
                    }
                    j++;
                }
            }
            actor2 = new QActor(table, turnsRemembered2, healthBarCutoff, manaBarCutoff);
        }
        roundRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasWinner && (ct1 == ControlType.Human || ct2 == ControlType.Human || battles >= 100))
        {
            combatText.gameObject.SetActive(true);
            if (player1.GetHealth() > 0) combatText.text = "Player 1 " + player1.GetUnitName() + " Wins!";
            else combatText.text = "Player 2 " + player2.GetUnitName() + " Wins!";
        }
        if (ct1 != ControlType.Human && ct2 != ControlType.Human &&  evaluating && !roundRunning && !hasWinner)
        {
            // AI vs AI
            roundRunning = true;
            if (ct1 == ControlType.GeneticPair || ct1 == ControlType.ReinforcementLearning) TakeTurnAI(GetTurnAction(player1, player2, ct1, 0), GetTurnAction(player2, player1, ct2, 1));
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
            case ControlType.GeneticSoloAdvancedTrainingPartner:
                if (playerTurn == 0) return GetActionFromInt(candidate1.GetCandidateAction(unit, enemy, th), unit);
                else return GetActionFromInt(candidate2.GetCandidateAction(unit, enemy, th), unit);
            case ControlType.GeneticSoloAdvancedHeuristic:
                if (playerTurn == 0) return GetActionFromInt(candidate1.GetCandidateAction(unit, enemy, th), unit);
                else return GetActionFromInt(candidate2.GetCandidateAction(unit, enemy, th), unit);
            case ControlType.ReinforcementLearning:
                return GetActionFromInt(training.GetAction(playerTurn, unit, enemy), unit);
            case ControlType.Reinforcement:
                if (playerTurn == 0) return GetActionFromInt(actor1.GetActorAction(unit, enemy, th), unit);
                else return GetActionFromInt(actor2.GetActorAction(unit, enemy, th), unit);
            case ControlType.ReinforcementVsAll:
                if (playerTurn == 0) return GetActionFromInt(actor1.GetActorAction(unit, enemy, th), unit);
                else return GetActionFromInt(actor2.GetActorAction(unit, enemy, th), unit);
        }
        return null;
    }

    public IEnumerator TakeTurn(Action unit1Action, Action unit2Action)
    {
        //if (ct1 != ControlType.GeneticPair) Debug.Log(roundNumber.text);
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
            if (hasWinner)
            {
                if (battles < 100 && ct1 != ControlType.Human && ct2 != ControlType.Human)
                {
                    ResetBattle();
                    Evaluate();
                }
                yield break;
            }
            Round(player2, player1, unit2Action);
            yield return new WaitForSeconds(waitTime);
            if (hasWinner)
            {
                if (battles < 100 && ct1 != ControlType.Human && ct2 != ControlType.Human)
                {
                    ResetBattle();
                    Evaluate();
                }
                yield break;
            }
        }
        else
        {
            Round(player2, player1, unit2Action);
            yield return new WaitForSeconds(waitTime);
            if (hasWinner) 
            {
                if (battles < 100 && ct1 != ControlType.Human && ct2 != ControlType.Human)
                {
                    ResetBattle();
                    Evaluate();
                }
                yield break; 
            }
            Round(player1, player2, unit1Action);
            yield return new WaitForSeconds(waitTime);
            if (hasWinner) 
            {
                if (battles < 100 && ct1 != ControlType.Human && ct2 != ControlType.Human)
                {
                    ResetBattle();
                    Evaluate();
                }
                yield break; 
            }

        }
        th.TickEffects(player1, player2, actionsUsed[0], actionsUsed[1]);
        if (player1.GetHealth() == 0)
        {
            Win(player2, player1);
            if (battles < 100 && ct1 != ControlType.Human && ct2 != ControlType.Human)
            {
                ResetBattle();
                Evaluate();
            }
        }
        else if (player2.GetHealth() == 0)
        {
            Win(player1, player2);
            if (battles < 100 && ct1 != ControlType.Human && ct2 != ControlType.Human)
            {
                ResetBattle();
                Evaluate();
            }
        }
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
        if (ct1 != ControlType.GeneticPair && ct1 != ControlType.ReinforcementLearning) Debug.Log("Round: " + round);
        if (ct1 == ControlType.Human || ct2 == ControlType.Human) playerSelect.SetActive(false);
        if (ct1 != ControlType.GeneticPair && ct1 != ControlType.ReinforcementLearning) combatText.gameObject.SetActive(true);
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
            if (hasWinner) { return; }

        }
        th.TickEffects(player1, player2, actionsUsed[0], actionsUsed[1]);
        if (player1.GetHealth() == 0)
        {
            Win(player2, player1);
        }
        else if (player2.GetHealth() == 0) 
        { 
            Win(player1, player2);
        }
        if (playerSelect)
        {
            if (ct1 == ControlType.Human || ct2 == ControlType.Human) playerSelect.SetActive(true);
            if (ct1 != ControlType.Human && ct2 == ControlType.Human) SetButtons(player2);
            else SetButtons(player1);
        }
        if (ct1 != ControlType.GeneticPair && ct1 != ControlType.ReinforcementLearning) combatText.gameObject.SetActive(false);
        round++;
        if (ct1 != ControlType.GeneticPair) roundNumber.text = "Round " + round;
        roundRunning = false;
        if (logging) combatLog += "\nHealth " + (int)(player1.GetHealthPercentage() * 100) + " || " + (int)(player2.GetHealthPercentage() * 100) + " Enemy Health\n";
    }

    public void Round(Unit caster, Unit enemy, Action action)
    {
        if (caster.GetMana() < action.GetManaCost())
        {
            if(logging) combatLog += "" + caster.name + " no longer has the mana to use " + action.GetActionName() + " || ";
            if (ct1 != ControlType.GeneticPair) combatText.text = "" + caster.name + " no longer has the mana to use " + action.GetActionName();
            // if (ct1 != ControlType.GeneticPair) Debug.Log(caster.name + " no longer has the mana to use " + action.GetActionName());
        }
        else if (th.CheckForEffect(caster, Effect.Stunned) == -1)
        {
            if (logging) combatLog += "" + caster.name + " used " + action.GetActionName() + " || ";
            if (ct1 != ControlType.GeneticPair) combatText.text = "" + caster.name + " used " + action.GetActionName();
            // if (ct1 != ControlType.GeneticPair) Debug.Log(caster.name + " used " + action.GetActionName());
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
            else if(round > 40)
            {
                hasWinner = true;
                evaluating = false;
            }
        }
        else
        {
            if (logging) combatLog += "" + caster.name + " is Stunned, and cannot move || ";
            if (ct1 != ControlType.GeneticPair) combatText.text = "" + caster.name + " is Stunned, and cannot move";
            // if (ct1 != ControlType.GeneticPair) Debug.Log(caster.name + " is Stunned, and cannot move");
        }
    }

    public void Win(Unit winner, Unit loser)
    {
        // if (ct1 != ControlType.GeneticPair) Debug.Log(winner.name + " Wins");
        if(logging) combatLog += "\nHealth " + (int)(player1.GetHealthPercentage() * 100) + " || " + (int)(player2.GetHealthPercentage() * 100) + " Enemy Health";
        th.ClearEffects();
        hasWinner = true;
        evaluating = false;
        logging = false;
        if (ct1 != ControlType.GeneticPair && ct1 != ControlType.ReinforcementLearning && ct1 != ControlType.Human && ct2 != ControlType.Human)
        {
            if (winner == player1) { wins[0]++; results[battles] = 1; }
            else if (winner == player2) { wins[1]++; results[battles] = 2; }
            // Debug.Log(player1.name + " wins: " + wins[0] + " | " + player2.name + " wins: " + wins[1]);
            battles++;
            if(battles == 100)
            {
                int wins1 = 0;
                int wins2 = 0;
                string res = player1.name + " wins: " + wins[0] + " | " + player2.name + " wins: " + wins[1] + "\n" + "Game Order:\n";
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i] == 1) wins1++;
                    else wins2++;
                    res += player1.name + " wins: " + wins1 + " | " + player2.name + " wins: " + wins2 + "\n";
                }
                Debug.Log(res);
                string p = Application.dataPath + "/Graphs/Results.csv";
                if(wins[0] + wins[1] != 100) File.AppendAllText(p, "50,50\n");
                else File.AppendAllText(p, wins[0] + "," + wins[1] + "\n");
                bool cont = true;
                count2++;
                if(count2 == ciel2)
                {
                    count1++;
                    count2 = 0;
                    if(count1 == ciel1)
                    {
                        cont = false;
                    }
                }
                if (cont)
                {
                    battles = 0;
                    wins = new int[2];
                    results = new int[100];
                    if (ct1 == ControlType.GeneticSoloBasic || ct1 == ControlType.GeneticSoloAdvanced || ct1 == ControlType.GeneticSoloAdvancedHeuristic)
                    {
                        string geneString;
                        string complexity = "";
                        if (ct1 == ControlType.GeneticSoloBasic) complexity = "Basic";
                        else if (ct1 == ControlType.GeneticSoloAdvanced) complexity = "Advanced";
                        else if (ct1 == ControlType.GeneticSoloAdvancedTrainingPartner) complexity = "Advanced+";
                        else if (ct1 == ControlType.GeneticSoloAdvancedHeuristic) complexity = "AdvancedHeuristic";
                        string path = Application.dataPath + "/Best Candidates/" + player1.GetUnitName() + "-" + player2.GetUnitName() + "-" + complexity + "-" + (count1 + 1);
                        if (ct1 == ControlType.GeneticSoloAdvanced || ct1 == ControlType.GeneticSoloAdvancedTrainingPartner || ct1 == ControlType.GeneticSoloAdvancedHeuristic) path += "-" + turnsRemembered1;
                        Debug.Log("Player 1: " + complexity + (ct1 != ControlType.GeneticSoloBasic ? (" " + turnsRemembered1) : "") + " Generation " + (count1 + 1));
                        StreamReader sr = new(path);
                        geneString = sr.ReadToEnd();
                        sr.Close();
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
                        candidate1 = new Candidate(genes, turnsRemembered1, healthBarCutoff, manaBarCutoff);
                    }
                    if (ct2 == ControlType.GeneticSoloBasic || ct2 == ControlType.GeneticSoloAdvanced || ct2 == ControlType.GeneticSoloAdvancedHeuristic)
                    {
                        string geneString;
                        string complexity = "";
                        if (ct2 == ControlType.GeneticSoloBasic) complexity = "Basic";
                        else if (ct2 == ControlType.GeneticSoloAdvanced) complexity = "Advanced";
                        else if (ct2 == ControlType.GeneticSoloAdvancedTrainingPartner) complexity = "Advanced+";
                        else if (ct2 == ControlType.GeneticSoloAdvancedHeuristic) complexity = "AdvancedHeuristic";
                        string path = Application.dataPath + "/Best Candidates/" + player2.GetUnitName() + "-" + player1.GetUnitName() + "-" + complexity + "-" + (count1 + 1);
                        if (ct2 == ControlType.GeneticSoloAdvanced || ct2 == ControlType.GeneticSoloAdvancedTrainingPartner || ct2 == ControlType.GeneticSoloAdvancedHeuristic) path += "-" + turnsRemembered2;
                        Debug.Log("Player 2: " + complexity + (ct2 != ControlType.GeneticSoloBasic ? (" " + turnsRemembered2) : "") + " Generation " + (count2 + 1));
                        StreamReader sr = new(path);
                        geneString = sr.ReadToEnd();
                        sr.Close();
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
                        candidate2 = new Candidate(genes, turnsRemembered2, healthBarCutoff, manaBarCutoff);
                    }

                }
            }
        }
    }

    public void Evaluate()
    {
        evaluating = true;
    }

    public void TestCurrentState()
    {
        logging = true;
        combatLog = "Health 100 || 100 Enemy Health\n";
    }

    public string GetCombatLog()
    {
        return combatLog;
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
