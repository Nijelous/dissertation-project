using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Training : MonoBehaviour
{
    [SerializeField]
    private Unit unit1;
    [SerializeField]
    private Unit unit2;
    [SerializeField]
    [Range(0, 1)]
    private float learningRate;
    [SerializeField]
    [Range(0, 1)]
    private float discountRate;
    [SerializeField]
    [Range(0, 3)]
    private int turnsRemembered;
    [SerializeField]
    [Range(0, 1)]
    private float healthBarCutoff;
    [SerializeField]
    [Range(0, 1)]
    private float manaBarCutoff;

    [SerializeField]
    private GameObject playerSelect;
    [SerializeField]
    private GameObject player1Bars;
    [SerializeField]
    private GameObject player2Bars;
    [SerializeField]
    private GameObject roundNumber;
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private Controller controller;
    [SerializeField]
    private TurnHandler th;
    private GameObject set;
    private int setNumber;
    private GameObject explorationRateGO;
    private float explorationRate;
    private GameObject winLossOverall;
    private int[] results;
    private GameObject winLossLast100;
    private List<int> last100;

    private Candidate partner;
    private QActor actor;

    private bool solutionFound;
    private bool testing;
    private int testingGames;
    private float lastTime;

    [SerializeField]
    private bool trainVsAll;
    [SerializeField]
    private GameObject fighter;
    [SerializeField]
    private GameObject rogue;
    [SerializeField]
    private GameObject wizard;
    [SerializeField]
    private GameObject player2;
    [SerializeField]
    private GameObject classes;
    private int masters;
    private int mastersBeaten;
    // Start is called before the first frame update
    void Start()
    {
        explorationRate = 1;
        setNumber = 1;
        testing = false;
        testingGames = 0;
        results = new int[2];
        last100 = new();
        lastTime = 0;
        mastersBeaten = 0;
        if (trainVsAll)
        {
            masters = 3;
            fighter.transform.parent = player2.transform;
            unit2 = fighter.GetComponent<Unit>();
            player2.GetComponent<UnitGraphics>().SetUnit(unit2);
        }
        else masters = 1;
        playerSelect.SetActive(false);
        unit1.gameObject.GetComponentInParent<SpriteRenderer>().enabled = false;
        unit2.gameObject.GetComponentInParent<SpriteRenderer>().enabled = false;
        unit1.gameObject.name = "Actor (" + unit1.GetUnitName() + ")";
        unit2.gameObject.name = "Trainer (" + unit2.GetUnitName() + ")";
        player1Bars.GetComponent<RectTransform>().localPosition = new Vector3(-150, 40, 0);
        player2Bars.GetComponent<RectTransform>().localPosition = new Vector3(150, 40, 0);
        GameObject vs = Instantiate(roundNumber);
        vs.transform.SetParent(canvas.transform);
        vs.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1);
        vs.GetComponent<RectTransform>().localPosition = new Vector3(0, -10, 0);
        vs.GetComponent<Text>().text = "VS";
        roundNumber.GetComponent<RectTransform>().localPosition = new Vector3(175, 200, 0);
        set = Instantiate(roundNumber);
        set.transform.SetParent(canvas.transform);
        set.GetComponent<RectTransform>().localPosition = new Vector3(-175, 200, 0);
        set.GetComponent<Text>().text = "Set " + setNumber;
        explorationRateGO = Instantiate(roundNumber);
        explorationRateGO.transform.SetParent(canvas.transform);
        explorationRateGO.GetComponent<RectTransform>().localPosition = new Vector3(-375, 175, 0);
        explorationRateGO.GetComponent<Text>().fontSize = 20;
        explorationRateGO.GetComponent<Text>().text = "Exploration Rate: " + explorationRate;
        explorationRateGO.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
        winLossOverall = Instantiate(roundNumber);
        winLossOverall.transform.SetParent(canvas.transform);
        winLossOverall.GetComponent<RectTransform>().localPosition = new Vector3(-375, 145, 0);
        winLossOverall.GetComponent<Text>().fontSize = 20;
        winLossOverall.GetComponent<Text>().text = "Win/Loss: " + results[0] + "-" + results[1];
        winLossOverall.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
        winLossLast100 = Instantiate(roundNumber);
        winLossLast100.transform.SetParent(canvas.transform);
        winLossLast100.GetComponent<RectTransform>().localPosition = new Vector3(-375, 115, 0);
        winLossLast100.GetComponent<Text>().fontSize = 20;
        winLossLast100.GetComponent<Text>().text = "Winrate Last 100: 0%";
        winLossLast100.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

        string path = Application.dataPath + "/Best Candidates/Training Partner " + unit1.GetUnitName() + "-" + unit2.GetUnitName();
        StreamReader sr = new(path);
        string geneString = sr.ReadToEnd();
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
        partner = new Candidate(genes, 1, healthBarCutoff, manaBarCutoff);

        actor = new QActor(turnsRemembered, healthBarCutoff, manaBarCutoff);
        controller.SetController(unit1, unit2, ControlType.ReinforcementLearning, ControlType.ReinforcementLearning, th, true, 0, new int[] { turnsRemembered, 0 }, healthBarCutoff, manaBarCutoff, this);
        controller.gameObject.SetActive(true);
    }

    public int GetAction(int player, Unit unit, Unit enemy)
    {
        if (player == 0)
        {
            float rand = Random.Range(0f, 1f);
            if (testing) rand = 1.1f;
            return actor.GetActorActionLearning(unit, enemy, th, rand < explorationRate);
        }
        else return partner.GetCandidateAction(unit, enemy, th);
    }

    // Update is called once per frame
    void Update()
    {
        if (!solutionFound)
        {
            if (controller.FinishedEvaluating(100))
            {
                actor.UpdateQTable(unit1, unit2, learningRate, discountRate);
                if (unit1.GetHealth() == 0)
                {
                    results[1]++;
                    if (explorationRate < 1) explorationRate = Mathf.Min(1f-(results[0]/10000f), explorationRate + 0.001f);
                    if (explorationRate < 0) explorationRate = 0;
                    last100.Add(1);
                }
                else
                {
                    results[0]++;
                    if (explorationRate > 0) explorationRate = Mathf.Max(0, explorationRate - 0.01f);
                    last100.Add(0);
                }
                if (last100.Count > 100) last100.RemoveAt(0);
                explorationRateGO.GetComponent<Text>().text = "Exploration Rate: " + explorationRate;
                winLossOverall.GetComponent<Text>().text = "Win/Loss: " + results[0] + "-" + results[1];
                float wins = 0;
                for(int i = 0; i < last100.Count; i++)
                {
                    if (last100[i] == 0) wins++;
                }
                winLossLast100.GetComponent<Text>().text = "Winrate Last 100: " + (wins/last100.Count)*100 + "%";
                setNumber++;
                set.GetComponent<Text>().text = "Set " + setNumber;
                if (testing)
                {
                    if (testingGames == 99) Debug.Log(controller.GetCombatLog());
                    testingGames++;
                    controller.TestCurrentState();
                }
                if (setNumber % 500 == 0)
                {
                    testing = true;
                    controller.TestCurrentState();
                    // actor.PrintQTable();
                }
                else if (testingGames == 100)
                {
                    testing = false;
                    Debug.Log("Last testing round: " + wins);
                    testingGames = 0;
                }
                if (wins >= 90)
                {
                    solutionFound = true;
                    mastersBeaten++;
                    if (mastersBeaten == masters)
                    {
                        Debug.Log("Solution Found For " + turnsRemembered + " Turns Remembered After " + setNumber + " Sets and " + (Time.time - lastTime) + " seconds");
                        lastTime = Time.time;
                    }
                    actor.PrintQTable();
                    SolutionFound();
                }
                else
                {
                    controller.ResetBattle();
                    controller.Evaluate();
                }
            }
        }
    }

    private void SolutionFound()
    {
        string path;
        if(trainVsAll) path = Application.dataPath + "/Best Actors/" + unit1.GetUnitName() + "-All-" + turnsRemembered;
        else path = Application.dataPath + "/Best Actors/" + unit1.GetUnitName() + "-" + unit2.GetUnitName() + "-" + turnsRemembered;
        if (mastersBeaten == masters)
        {
            File.WriteAllText(path, actor.GetQTable());
            path = Application.dataPath + "/Results/" + unit1.GetUnitName() + "-" + unit2.GetUnitName() + "-" + (trainVsAll ? "Q+" : "Q");
            if (File.Exists(path))
            {
                File.AppendAllText(path, "Episodes: " + setNumber + " Time Taken: " + lastTime + " " + turnsRemembered + "\n");
            }
            else
            {
                path = Application.dataPath + "/Results/" + unit2.GetUnitName() + "-" + unit1.GetUnitName() + "-" + (trainVsAll ? "Q+" : "Q");
                if (File.Exists(path))
                {
                    File.AppendAllText(path, "Episodes: " + setNumber + " Time Taken: " + lastTime + " " + turnsRemembered + "\n");
                }
                else
                {
                    File.WriteAllText(path, "Episodes: " + setNumber + " Time Taken: " + lastTime + " " + turnsRemembered + "\n");
                }
            }

        }
        if (trainVsAll)
        {
            explorationRate = Mathf.Min(explorationRate + 0.5f, 1);
            unit2.gameObject.name = unit2.GetUnitName();
            unit2.transform.parent = classes.transform;
            switch (unit2.GetUnitName())
            {
                case "Fighter":
                    rogue.transform.parent = player2.transform;
                    unit2 = rogue.GetComponent<Unit>();
                    player2.GetComponent<UnitGraphics>().SetUnit(unit2);
                    break;
                case "Rogue":
                    wizard.transform.parent = player2.transform;
                    unit2 = wizard.GetComponent<Unit>();
                    player2.GetComponent<UnitGraphics>().SetUnit(unit2);
                    break;
                case "Wizard":
                    fighter.transform.parent = player2.transform;
                    unit2 = fighter.GetComponent<Unit>();
                    player2.GetComponent<UnitGraphics>().SetUnit(unit2);
                    break;
            }
            unit2.gameObject.name = "Trainer (" + unit2.GetUnitName() + ")";
            path = Application.dataPath + "/Best Candidates/Training Partner " + unit1.GetUnitName() + "-" + unit2.GetUnitName();
            StreamReader sr = new(path);
            string geneString = sr.ReadToEnd();
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
            partner = new Candidate(genes, 1, healthBarCutoff, manaBarCutoff);
            controller.SetController(unit1, unit2, ControlType.ReinforcementLearning, ControlType.ReinforcementLearning, th, true, 0, new int[] { turnsRemembered, 0 }, healthBarCutoff, manaBarCutoff, this);
        }
        if(turnsRemembered < 3 && masters == mastersBeaten)
        {
            explorationRate = 1;
            setNumber = 1;
            set.GetComponent<Text>().text = "Set " + setNumber;
            testing = false;
            testingGames = 0;
            results = new int[2];
            last100.Clear();
            mastersBeaten = 0;
            turnsRemembered++;
            actor = new QActor(turnsRemembered, healthBarCutoff, manaBarCutoff);
            solutionFound = false;
            controller.ResetBattle();
            controller.Evaluate();
        }
        else if (turnsRemembered < 4 && trainVsAll && mastersBeaten < masters)
        {
            last100.Clear();
            solutionFound = false;
            controller.ResetBattle();
            controller.Evaluate();
        }
    }
}
