using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Evolution : MonoBehaviour
{
    private List<Candidate> candidatesP1;
    private List<Candidate> candidatesP2;
    private List<float> fitness;
    [SerializeField]
    private int startPopulation = 2;
    [SerializeField]
    private bool showEvolution;
    [SerializeField]
    private CandidateType candidateType;
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private GameObject playerSelect;
    [SerializeField]
    private GameObject player1Bars;
    [SerializeField] 
    private GameObject player2Bars;
    [SerializeField]
    private GameObject roundNumber;
    private GameObject generationNumber;
    [SerializeField]
    private GameObject player1;
    [SerializeField]
    private GameObject player2;
    [SerializeField]
    private Unit unit1;
    [SerializeField]
    private Unit unit2;
    [SerializeField]
    private GameObject setPrefab;
    [SerializeField]
    private bool quickCombat;
    [SerializeField]
    private float waitTime;
    private List<GameObject> sets;
    private List<GameObject> evaluatingSets;
    [SerializeField]
    private int maxRounds;
    private bool started = false;
    private bool generationFinished = true;
    [SerializeField]
    private int mutationValue;
    [SerializeField]
    private const float mutationChance = 0.001f;
    private int generationCount = 1;
    private GameObject[] averageWeights;
    private int[] initialAverageWeights;
    private int[] lastAverageWeights;
    private List<int> totalAverageChanges;
    private bool bestFound = false;
    [SerializeField]
    private GameObject dropdownMenu;
    [SerializeField]
    private int advancedTurnsRemembered;
    // Start is called before the first frame update
    void Awake()
    {
        candidatesP1 = new List<Candidate>();
        candidatesP2 = new List<Candidate>();
        sets = new List<GameObject>();
        evaluatingSets = new List<GameObject>();
        fitness = new List<float>(new float[startPopulation]);
        GenerateCandidates(candidatesP1);
        GenerateCandidates(candidatesP2);
        int weightLength = candidatesP1[0].GetWeightsLength();
        if (showEvolution)
        {
            playerSelect.SetActive(false);
            player1.GetComponent<SpriteRenderer>().enabled = false;
            player2.GetComponent<SpriteRenderer>().enabled = false;
            if (candidateType == CandidateType.Basic)
            {
                for (int i = 0; i < startPopulation / 2; i++)
                {
                    GameObject parent = Instantiate(setPrefab, canvas.transform);
                    parent.name = "Set " + i;
                    parent.transform.localPosition = new Vector3(0, 0, 0);
                    sets.Add(parent);
                    GameObject bar1 = Instantiate(player1Bars);
                    bar1.transform.SetParent(parent.transform);
                    bar1.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1);
                    bar1.GetComponent<RectTransform>().localPosition = new Vector3(-525 + 228 * (i % 5), 200 - 45 * (i / 5), 0);
                    GameObject bar2 = Instantiate(player2Bars);
                    bar2.transform.SetParent(parent.transform);
                    bar2.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1);
                    bar2.GetComponent<RectTransform>().localPosition = new Vector3(-415 + 228 * (i % 5), 200 - 45 * (i / 5), 0);
                    GameObject vs = Instantiate(roundNumber);
                    vs.transform.SetParent(parent.transform);
                    vs.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1);
                    vs.GetComponent<RectTransform>().localPosition = new Vector3(-470 + 228 * (i % 5), 180 - 45 * (i / 5), 0);
                    vs.GetComponent<Text>().text = "VS";
                }
            }
            else if(candidateType == CandidateType.Advanced)
            {
                dropdownMenu.SetActive(true);
                GameObject parent = Instantiate(setPrefab, canvas.transform);
                parent.name = "Set 0";
                parent.transform.localPosition = new Vector3(0, 0, 0);
                sets.Add(parent);
                GameObject bar1 = Instantiate(player1Bars);
                bar1.transform.SetParent(parent.transform);
                bar1.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1);
                bar1.GetComponent<RectTransform>().localPosition = new Vector3(500, 250, 0);
                GameObject bar2 = Instantiate(player2Bars);
                bar2.transform.SetParent(parent.transform);
                bar2.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1);
                bar2.GetComponent<RectTransform>().localPosition = new Vector3(500, 190, 0);
                GameObject vs = Instantiate(roundNumber);
                vs.transform.SetParent(parent.transform);
                vs.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1);
                vs.GetComponent<RectTransform>().localPosition = new Vector3(500, 215, 0);
                vs.GetComponent<Text>().text = "VS";
                for (int i = 1; i < startPopulation / 2; i++)
                {
                    parent = Instantiate(setPrefab, canvas.transform);
                    parent.name = "Set " + i;
                    sets.Add(parent);
                    bar1 = new();
                    bar1.transform.parent = parent.transform;
                    bar2 = new();
                    bar2.transform.parent = parent.transform;
                }
            }
            roundNumber.transform.localPosition = candidateType == CandidateType.Basic ? new Vector3(123, 234, 0) : new Vector3(500, 150, 0);
            if (candidateType == CandidateType.Advanced) roundNumber.GetComponent<Text>().fontSize = 25;
            generationNumber = Instantiate(roundNumber, canvas.transform);
            generationNumber.transform.localPosition = candidateType == CandidateType.Basic ? new Vector3(-123, 234, 0) : new Vector3(500, 125, 0);
            if (candidateType == CandidateType.Advanced) generationNumber.GetComponent<Text>().fontSize = 20;
            generationNumber.GetComponent<Text>().text = "Generation " + generationCount;
            player1Bars.SetActive(false);
            player2Bars.SetActive(false);
            averageWeights = new GameObject[weightLength*2];
            initialAverageWeights = new int[weightLength*2];
            lastAverageWeights = new int[weightLength*2];
            for(int i = 0; i < initialAverageWeights.Length; i++)
            {
                averageWeights[i] = Instantiate(roundNumber, canvas.transform);
                averageWeights[i].name = "AverageWeights " + i;
                averageWeights[i].tag = "Weight";
                if (candidateType == CandidateType.Basic)
                {
                    averageWeights[i].GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                    if (i % 13 < 7) averageWeights[i].GetComponent<RectTransform>().localPosition = new Vector3(-480 + ((i % 13) * 30), 253 - ((i / 13) * 25), 0);
                    else averageWeights[i].GetComponent<RectTransform>().localPosition = new Vector3(205 + (((i - 7) % 13) * 30), 253 - ((i / 13) * 25), 0);
                    averageWeights[i].GetComponent<Text>().text = "0";
                    averageWeights[i].GetComponent<Text>().fontSize = 20;
                }
                else if(candidateType == CandidateType.Advanced)
                {
                    int j = i % weightLength;
                    averageWeights[i].GetComponent<RectTransform>().sizeDelta = new Vector2(12, 11);
                    averageWeights[i].GetComponent<RectTransform>().localPosition = new Vector3(-565.5f + (((j%70) * 13) + (((j%70) / 7) * 9)), 261 - ((j / 70) * 11), 0);
                    averageWeights[i].GetComponent<Text>().text = "0";
                    averageWeights[i].GetComponent<Text>().fontSize = i < weightLength ? 10 : 11;
                }
            }
            totalAverageChanges = new List<int>();
            if (candidateType == CandidateType.Basic)
            {
                for (int i = 0; i < 6; i++)
                {
                    GameObject typeDisplay = Instantiate(roundNumber, canvas.transform);
                    typeDisplay.name = "TypeDisplay " + i;
                    typeDisplay.GetComponent<RectTransform>().sizeDelta = new Vector2(45, 16);
                    typeDisplay.GetComponent<RectTransform>().localPosition = new Vector3(456 + ((i % 3) * 45), 253 - ((i / 3) * 16), 0);
                    switch (i)
                    {
                        case 0: typeDisplay.GetComponent<Text>().text = "DMG"; break;
                        case 1: typeDisplay.GetComponent<Text>().text = "HEAL"; break;
                        case 2: typeDisplay.GetComponent<Text>().text = "MANA"; break;
                        case 3: typeDisplay.GetComponent<Text>().text = "BUFF"; break;
                        case 4: typeDisplay.GetComponent<Text>().text = "STAT"; break;
                        case 5: typeDisplay.GetComponent<Text>().text = "USED"; break;
                    }
                    typeDisplay.GetComponent<Text>().fontSize = 14;
                }
            }
        }
        else 
        {
            for (int i = 0; i < startPopulation / 2; i++)
            {
                GameObject parent = Instantiate(setPrefab, canvas.transform);
                parent.name = "Set " + i;
                sets.Add(parent);
                GameObject bar1 = new();
                bar1.transform.parent = parent.transform;
                GameObject bar2 = new();
                bar2.transform.parent = parent.transform;
            }
            averageWeights = new GameObject[] {Instantiate(roundNumber, canvas.transform),  Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform),
                Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform),
                Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform),  Instantiate(roundNumber, canvas.transform),
                Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform),
                Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform),
                Instantiate(roundNumber, canvas.transform),  Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform),
                Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform),
                Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform),  Instantiate(roundNumber, canvas.transform),
                Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform) };
            initialAverageWeights = new int[26];
            lastAverageWeights = new int[26];
            for(int i = 0; i < initialAverageWeights.Length; i++)
            {
                averageWeights[i].GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                averageWeights[i].tag = "Set";
                averageWeights[i].GetComponent<Text>().text = "0";
            }
            totalAverageChanges = new List<int>();
            waitTime = 0;
        }
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        foreach (GameObject set in sets)
        {
            Unit u1 = set.transform.GetChild(0).gameObject.AddComponent<Unit>();
            u1.SetAttributes(unit1);
            if(showEvolution && (candidateType == CandidateType.Basic || set.name == "Set 0")) set.transform.GetChild(0).gameObject.AddComponent<UnitGraphics>();
            Unit u2 = set.transform.GetChild(1).gameObject.AddComponent<Unit>();
            u2.SetAttributes(unit2);
            if (showEvolution && (candidateType == CandidateType.Basic || set.name == "Set 0")) set.transform.GetChild(1).gameObject.AddComponent<UnitGraphics>();
            set.GetComponent<Controller>().SetController(u1, u2, ControlType.GeneticPair, ControlType.GeneticPair, set.GetComponent<TurnHandler>(), quickCombat, waitTime);
        }
        if (showEvolution)
        {
            for (int i = 0; i < 14; i++)
            {
                GameObject display = Instantiate(roundNumber, canvas.transform);
                display.name = "ActionDisplay " + i;
                display.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 20);
                display.GetComponent<RectTransform>().localPosition = new Vector3(-450 + ((i % 7) * 150), -237 - ((i / 7) * 20), 0);
                display.GetComponent<Text>().fontSize = 18;
                if (i < 7)
                {
                    display.GetComponent<Text>().text = (i % 7) > 2 ?
                        unit1.GetMagicalActions().ToArray()[i - 3].GetActionName() : unit1.GetPhysicalActions().ToArray()[i].GetActionName();
                }
                if (i >= 7)
                {
                    display.GetComponent<Text>().text = (i % 7) > 2 ?
                        unit2.GetMagicalActions().ToArray()[i - 10].GetActionName() : unit2.GetPhysicalActions().ToArray()[i - 7].GetActionName();
                }
            }
        }
        started = true;
    }

    private void GenerateCandidates(List<Candidate> candidates)
    {
        for(int i = 0; i < startPopulation/2; i++)
        {
            candidates.Add(new Candidate(candidateType, advancedTurnsRemembered));
        }
    }

    private void FindFitness()
    {
        for(int i = 0; i < sets.Count; i++)
        {
            Unit u1 = sets[i].transform.GetChild(0).GetComponent<Unit>();
            Unit u2 = sets[i].transform.GetChild(1).GetComponent<Unit>();
            if (u1.GetHealth() == 0)
            {
                fitness[i] = 0;
                fitness[i + (startPopulation/2)] = 100;
            }
            else if(u2.GetHealth() == 0) {
                fitness[i + (startPopulation / 2)] = 0;
                fitness[i] = 100;
            }
            else
            {
                fitness[i] = u1.GetHealth() - u2.GetHealth();
                fitness[i + (startPopulation / 2)] = u2.GetHealth() - u1.GetHealth();
            }
        }
    }

    private void SortPopulation()
    {
        for (int i = 0; i < candidatesP1.Count; i++)
        {
            if (fitness[i] == 0)
            {
                candidatesP1.RemoveAt(i);
                fitness.RemoveAt(i);
                i--;
            }
        }
        QuickSort(0, candidatesP1.Count-1);
        for (int i = 0; i < candidatesP2.Count; i++)
        {
            if (fitness[i+candidatesP1.Count] == 0)
            {
                candidatesP2.RemoveAt(i);
                fitness.RemoveAt(i+candidatesP1.Count);
                i--;
            }
        }
        QuickSort(candidatesP1.Count, candidatesP2.Count - 1);
    }

    private void QuickSort(int left, int right)
    {
        if(left < right)
        {
            int pivot = Partition(left, right);

            if(pivot > 1) QuickSort(left, pivot - 1);

            if(pivot + 1 < right) QuickSort(pivot + 1, right);
        }
    }

    private int Partition(int left, int right)
    {
        float pivot = fitness[left];
        while (true)
        {
            while (fitness[left] < pivot) left++;

            while (fitness[right] > pivot) right--;

            if (left < right)
            {
                if (fitness[left] == fitness[right]) return right;
                (fitness[right], fitness[left]) = (fitness[left], fitness[right]);
                if(right < candidatesP1.Count) (candidatesP1[right], candidatesP1[left]) = (candidatesP1[left], candidatesP1[right]);
                else (candidatesP2[right - (candidatesP1.Count)], candidatesP2[left - (candidatesP1.Count)]) = 
                        (candidatesP2[left - (candidatesP1.Count)], candidatesP2[right - (candidatesP1.Count)]);
            }
            else return right;
        }
    }

    private void Reproduce(List<Candidate> candidates)
    {
        List<Candidate> oldCandidates = new List<Candidate>(candidates);
        candidates.Clear();
        if(oldCandidates.Count == 0)
        {
            GenerateCandidates(candidates);
            return;
        }
        int reverseCount = oldCandidates.Count-1;
        for(int i = 0; i < startPopulation/2; i++)
        {
            switch (Random.Range(0f, 1f))
            {
                case mutationChance:
                    if (reverseCount == 0)
                    {
                        candidates.Add(new Candidate(oldCandidates[reverseCount], oldCandidates[oldCandidates.Count - 1], mutationValue));
                        reverseCount = oldCandidates.Count;
                    }
                    else candidates.Add(new Candidate(oldCandidates[reverseCount], oldCandidates[reverseCount - 1], mutationValue));
                    break;
                default:
                    if (reverseCount == 0)
                    {
                        candidates.Add(new Candidate(oldCandidates[reverseCount], oldCandidates[oldCandidates.Count - 1]));
                        reverseCount = oldCandidates.Count;
                    }
                    else candidates.Add(new Candidate(oldCandidates[reverseCount], oldCandidates[reverseCount - 1]));
                    break;
            }
            reverseCount--;
        }
    }

    private void GetAverageWeights()
    {
        int currentAverageWeightsTotal = 0;
        int previousAverageWeightsTotal = 0;
        for(int i = 0; i < averageWeights.Length; i++)
        {
            previousAverageWeightsTotal += int.Parse(averageWeights[i].GetComponent<Text>().text);
            List<Candidate> candidates;
            candidates = i < 13 ? candidatesP1 : candidatesP2;
            int total = 0;
            for(int j = 0; j < startPopulation/2; j++)
            {
                total += candidates[j].GetWeight(i%13);
            }
            total /= startPopulation/2;
            currentAverageWeightsTotal += total;
            if (int.Parse(averageWeights[i].GetComponent<Text>().text) == 0) initialAverageWeights[i] = total;
            else
            {
                averageWeights[i].GetComponent<Text>().color = new Color
                    (total < initialAverageWeights[i] ? (float)(initialAverageWeights[i] - total) / initialAverageWeights[i] : 0,
                    total > initialAverageWeights[i] ? (float)(total - initialAverageWeights[i]) / (99 - initialAverageWeights[i]) : 0, 0);
                averageWeights[i].GetComponent<Text>().text = total.ToString();
                lastAverageWeights[i] = total;
            }
            averageWeights[i].GetComponent<Text>().text = total.ToString();
        }

        totalAverageChanges.Add(Mathf.Abs(currentAverageWeightsTotal - previousAverageWeightsTotal));
        int averageLastFive = 0;
        foreach(int i in totalAverageChanges)
        {
            averageLastFive += i;
        }
        if (averageLastFive <= 1)
        {
            bestFound = true;
        }
        else if(totalAverageChanges.Count > 4)
        {
            totalAverageChanges.RemoveAt(0);
        }
    }

    private void Evaluate()
    {
        int i = 0;
        foreach(GameObject set in sets)
        {
            evaluatingSets.Add(set);
            if (showEvolution)
            {
                if (candidateType == CandidateType.Basic)
                {
                    set.transform.GetChild(0).name = candidatesP1[i].GetWeightsAsString();
                    set.transform.GetChild(1).name = candidatesP2[i++].GetWeightsAsString();
                }
                else
                {
                    set.transform.GetChild(0).name = unit1.GetUnitName() + " 1";
                    set.transform.GetChild(1).name = unit2.GetUnitName() + " 2";
                }
            }
            set.GetComponent<Controller>().ResetBattle();
            set.GetComponent<Controller>().Evaluate();
            generationFinished = false;
        }
    }

    public int GetCandidateAction(int set, int unit, Unit u, Unit enemy, TurnHandler th)
    {
        List<Candidate> candidates = new List<Candidate>();
        if (unit == 0) candidates = candidatesP1;
        else if (unit == 1) candidates = candidatesP2;
        if(set < 50 && unit < 2)
        {
            return candidates[set].GetCandidateAction(u, enemy, th);
        }
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (started && !bestFound)
        {
            if (evaluatingSets.Count > 0)
            {
                if (showEvolution)
                {
                    roundNumber.GetComponent<Text>().text = "Round " + evaluatingSets[0].GetComponent<Controller>().GetRound();
                }
                for(int i = 0; i <= evaluatingSets.Count; i++)
                {
                    if (evaluatingSets[i].GetComponent<Controller>().FinishedEvaluating(maxRounds))
                    {
                        evaluatingSets.Remove(evaluatingSets[i]);
                        if (evaluatingSets.Count != 0)
                            i--;
                        
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if(generationFinished)
            {
                GetAverageWeights();
                if (!bestFound)
                {
                    Evaluate();
                }
                else
                {
                    BestFound();
                }
            }
            else
            {
                FindFitness();
                SortPopulation();
                Reproduce(candidatesP1);
                Reproduce(candidatesP2);
                fitness = new List<float>(new float[startPopulation]);
                generationCount++;
                if (showEvolution)
                {
                    generationNumber.GetComponent<Text>().text = "Generation " + generationCount;
                }
                generationFinished = true;
            }
        }
    }

    private void BestFound()
    {
        if (candidateType == CandidateType.Basic)
        {
            Debug.Log("Player 1 Weights: " + unit1.GetPhysicalActions().ToArray()[0].GetActionName() + " " + lastAverageWeights[0] + " | "
                + unit1.GetPhysicalActions().ToArray()[1].GetActionName() + " " + lastAverageWeights[1] + " | "
                + unit1.GetPhysicalActions().ToArray()[2].GetActionName() + " " + lastAverageWeights[2] + " | "
                + unit1.GetMagicalActions().ToArray()[0].GetActionName() + " " + lastAverageWeights[3] + " | "
                + unit1.GetMagicalActions().ToArray()[1].GetActionName() + " " + lastAverageWeights[4] + " | "
                + unit1.GetMagicalActions().ToArray()[2].GetActionName() + " " + lastAverageWeights[5] + " | "
                + unit1.GetMagicalActions().ToArray()[3].GetActionName() + " " + lastAverageWeights[6] + " | "
                + "Damage Priority" + " " + lastAverageWeights[7] + " | "
                + "Healing Priority" + " " + lastAverageWeights[8] + " | "
                + "Mana Regen Priority" + " " + lastAverageWeights[9] + " | "
                + "Buff Priority" + " " + lastAverageWeights[10] + " | "
                + "Status Effect Priority" + " " + lastAverageWeights[11] + " | "
                + "Applied Effect Priority" + " " + lastAverageWeights[12] + " | ");

            Debug.Log("Player 2 Weights: " + unit2.GetPhysicalActions().ToArray()[0].GetActionName() + " " + lastAverageWeights[13] + " | "
                + unit2.GetPhysicalActions().ToArray()[1].GetActionName() + " " + lastAverageWeights[14] + " | "
                + unit2.GetPhysicalActions().ToArray()[2].GetActionName() + " " + lastAverageWeights[15] + " | "
                + unit2.GetMagicalActions().ToArray()[0].GetActionName() + " " + lastAverageWeights[16] + " | "
                + unit2.GetMagicalActions().ToArray()[1].GetActionName() + " " + lastAverageWeights[17] + " | "
                + unit2.GetMagicalActions().ToArray()[2].GetActionName() + " " + lastAverageWeights[18] + " | "
                + unit2.GetMagicalActions().ToArray()[3].GetActionName() + " " + lastAverageWeights[19] + " | "
                + "Damage Priority" + " " + lastAverageWeights[20] + " | "
                + "Healing Priority" + " " + lastAverageWeights[21] + " | "
                + "Mana Regen Priority" + " " + lastAverageWeights[22] + " | "
                + "Buff Priority" + " " + lastAverageWeights[23] + " | "
                + "Status Effect Priority" + " " + lastAverageWeights[24] + " | "
                + "Applied Effect Priority" + " " + lastAverageWeights[25] + " | ");
        }
        else
        {
            Debug.Log("Done");
        }
        string path1;
        string path2;
        string complexity = "";
        if (candidateType == CandidateType.Basic) complexity = "Basic";
        else if (candidateType == CandidateType.Advanced) complexity = "Advanced";
        if (unit1.GetUnitName() == unit2.GetUnitName())
        {
            path1 = Application.dataPath + "/Best Candidates/" + unit1.GetUnitName() + "-" + unit2.GetUnitName() + "-" + complexity + "-1";
            path2 = Application.dataPath + "/Best Candidates/" + unit2.GetUnitName() + "-" + unit1.GetUnitName() + "-" + complexity + "-2";
        }
        else
        {
            path1 = Application.dataPath + "/Best Candidates/" + unit1.GetUnitName() + "-" + unit2.GetUnitName() + "-" + complexity;
            path2 = Application.dataPath + "/Best Candidates/" + unit2.GetUnitName() + "-" + unit1.GetUnitName() + "-" + complexity;
        }
        File.WriteAllText(path1, GetBestWeightsAsString(0));
        File.WriteAllText(path2, GetBestWeightsAsString(1));
    }

    private string GetBestWeightsAsString(int candidate)
    {
        string res = "";
        for (int i = 0; i < lastAverageWeights.Length / 2; i++)
        {
            res += lastAverageWeights[i + (candidate*(lastAverageWeights.Length/2))] + " ";
        }
        return res;
    }

    public bool HasFoundBest()
    {
        return bestFound;
    }

    public Candidate GetBestCandidate(int i)
    {
        int[] weights = new int[lastAverageWeights.Length/2];
        if(i == 0)
        {
            for(int j = 0; j < weights.Length; j++)
            {
                weights[j] = lastAverageWeights[j];
            }
        }
        else if(i == 1)
        {
            for (int j = 0; j < weights.Length; j++)
            {
                weights[j] = lastAverageWeights[j+weights.Length];
            }
        }
        return new Candidate(weights);
    }

    public void EvolutionSet(Unit unit1, Unit unit2)
    {
        this.unit1 = unit1;
        this.unit2 = unit2;
        showEvolution = false;
        quickCombat = true;
    }
}
