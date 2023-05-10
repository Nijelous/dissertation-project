using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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
    private GameObject changeNumber;
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
    private bool pairTraining;
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
    [Range(0, 1)]
    private float mutationChance = 0.005f;
    private int generationCount = 1;
    private GameObject[] averageWeights;
    private int[] initialAverageWeights;
    private int[] lastAverageWeights;
    private int lastTotalWeights;
    private List<int> totalAverageChanges;
    private List<int> allAverageChanges;
    private bool bestFound = false;
    [SerializeField]
    private GameObject dropdownMenu;
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
    private int count;
    private float lastTime;
    [SerializeField]
    [Range(1, 3)]
    private int endValue;
    private int geneLength;
    private int loserPityGeneration1 = 0;
    private int loserPityGeneration2 = 0;
    private int survivorCount1;
    private int survivorCount2;
    // Start is called before the first frame update
    void Awake()
    {
        lastTime = 0;
        candidatesP1 = new List<Candidate>();
        candidatesP2 = new List<Candidate>();
        sets = new List<GameObject>();
        evaluatingSets = new List<GameObject>();
        if(pairTraining) fitness = new List<float>(new float[startPopulation]);
        else fitness = new List<float>(new float[startPopulation/2]);
        if (pairTraining) survivorCount2 = 1;
        GenerateCandidates(candidatesP1);
        if(pairTraining) GenerateCandidates(candidatesP2);
        lastTotalWeights = 0;
        geneLength = candidatesP1[0].GetWeightsLength();
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
            else if(candidateType == CandidateType.Advanced || candidateType == CandidateType.AdvancedTrainingPartner || candidateType == CandidateType.AdvancedHeuristic)
            {
                if(turnsRemembered == 1) dropdownMenu.SetActive(true);
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
            if (candidateType == CandidateType.Advanced || candidateType == CandidateType.AdvancedTrainingPartner || candidateType == CandidateType.AdvancedHeuristic) roundNumber.GetComponent<Text>().fontSize = 25;
            generationNumber = Instantiate(roundNumber, canvas.transform);
            generationNumber.transform.localPosition = candidateType == CandidateType.Basic ? new Vector3(-123, 234, 0) : new Vector3(500, 125, 0);
            if (candidateType == CandidateType.Advanced || candidateType == CandidateType.AdvancedTrainingPartner || candidateType == CandidateType.AdvancedHeuristic) generationNumber.GetComponent<Text>().fontSize = 20;
            generationNumber.GetComponent<Text>().text = "Generation " + generationCount;
            player1Bars.SetActive(false);
            player2Bars.SetActive(false);
            if (turnsRemembered <= 1 || candidateType == CandidateType.Basic)
            {
                averageWeights = new GameObject[geneLength * 2];
                initialAverageWeights = new int[geneLength * 2];
                for (int i = 0; i < initialAverageWeights.Length; i++)
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
                    else if (candidateType == CandidateType.Advanced || candidateType == CandidateType.AdvancedTrainingPartner || candidateType == CandidateType.AdvancedHeuristic)
                    {
                        int j = i % geneLength;
                        averageWeights[i].GetComponent<RectTransform>().sizeDelta = new Vector2(12, 11);
                        averageWeights[i].GetComponent<RectTransform>().localPosition = new Vector3(-565.5f + (((j % 70) * 13) + (((j % 70) / 7) * 9)), 261 - ((j / 70) * 11), 0);
                        averageWeights[i].GetComponent<Text>().text = "0";
                        averageWeights[i].GetComponent<Text>().fontSize = i < geneLength ? 10 : 11;
                    }
                }
            }
            else
            {
                averageWeights = new GameObject[5];
                for (int i = 0; i < averageWeights.Length; i++)
                {
                    averageWeights[i] = Instantiate(roundNumber, canvas.transform);
                    averageWeights[i].name = "AverageWeights " + i;
                    averageWeights[i].tag = "Weight";
                    averageWeights[i].GetComponent<RectTransform>().localPosition = new Vector3(0, 200 - (i * 50), 0);
                    averageWeights[i].GetComponent<Text>().text = "0";
                }
            }
            lastAverageWeights = new int[geneLength * 2];
            totalAverageChanges = new List<int>();
            allAverageChanges = new List<int>();
            changeNumber = Instantiate(roundNumber, canvas.transform);
            changeNumber.GetComponent<Text>().text = "0";
            if ((candidateType == CandidateType.Advanced || candidateType == CandidateType.AdvancedTrainingPartner || candidateType == CandidateType.AdvancedHeuristic) && turnsRemembered <= 1)
            {
                changeNumber.transform.localPosition = new Vector3(500, 100, 0);
                changeNumber.GetComponent<Text>().fontSize = 15;
            }
            else
            {
                changeNumber.GetComponent<Text>().fontSize = 100;
            }
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
        if (!pairTraining)
        {
            string path = Application.dataPath + "/Best Candidates/Training Partner " + unit1.GetUnitName() + "-" + unit2.GetUnitName();
            Debug.Log(path);
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
            candidatesP2.Add(new Candidate(genes, 1, healthBarCutoff, manaBarCutoff));
        }
        foreach (GameObject set in sets)
        {
            Unit u1 = set.transform.GetChild(0).gameObject.AddComponent<Unit>();
            u1.SetAttributes(unit1);
            if(showEvolution && (candidateType == CandidateType.Basic || set.name == "Set 0")) set.transform.GetChild(0).gameObject.AddComponent<UnitGraphics>();
            Unit u2 = set.transform.GetChild(1).gameObject.AddComponent<Unit>();
            u2.SetAttributes(unit2);
            if (showEvolution && (candidateType == CandidateType.Basic || set.name == "Set 0")) set.transform.GetChild(1).gameObject.AddComponent<UnitGraphics>();
            set.GetComponent<Controller>().SetController(u1, u2, ControlType.GeneticPair, ControlType.GeneticPair, set.GetComponent<TurnHandler>(), true, waitTime, new int[] {turnsRemembered, turnsRemembered}, healthBarCutoff, manaBarCutoff);
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
            candidates.Add(new Candidate(candidateType, turnsRemembered, healthBarCutoff, manaBarCutoff));
        }
    }

    private void FindFitness()
    {
        for(int i = 0; i < sets.Count; i++)
        {
            if (pairTraining)
            {
                Unit u1 = sets[i].transform.GetChild(0).GetComponent<Unit>();
                Unit u2 = sets[i].transform.GetChild(1).GetComponent<Unit>();
                if (u1.GetHealth() == 0)
                {
                    fitness[i] = 0;
                    fitness[i + (startPopulation / 2)] = 100;
                }
                else if (u2.GetHealth() == 0)
                {
                    fitness[i + (startPopulation / 2)] = 0;
                    fitness[i] = 100;
                }
                else
                {
                    fitness[i] = 0;
                    fitness[i + (startPopulation / 2)] = 0;
                }
            }
            else
            {
                Unit u1 = sets[i].transform.GetChild(0).GetComponent<Unit>();
                Unit u2 = sets[i].transform.GetChild(1).GetComponent<Unit>();
                if (u2.GetHealth() == 0)
                {
                    fitness[i] = u1.GetHealth();
                }
                else 
                {
                    fitness[i] = 0;
                }
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
        survivorCount1 += candidatesP1.Count;
        survivorCount1 /= 2;
        if (pairTraining)
        {
            for (int i = 0; i < candidatesP2.Count; i++)
            {
                if (fitness[i + candidatesP1.Count] == 0)
                {
                    candidatesP2.RemoveAt(i);
                    fitness.RemoveAt(i + candidatesP1.Count);
                    i--;
                }
            }
            QuickSort(candidatesP1.Count, candidatesP2.Count - 1);
            survivorCount2 += candidatesP2.Count;
            survivorCount2 /= 2;
        }
        Debug.Log("Candidate 1s Remaining: " + candidatesP1.Count + " Candidate 2s Remaining: " + candidatesP2.Count);
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

    private void Reproduce(List<Candidate> candidates, int candidate)
    {
        List<Candidate> oldCandidates = new(candidates);
        candidates.Clear();
        if(oldCandidates.Count <= 1 || ((candidate == 1 ? survivorCount1 : survivorCount2) < (pairTraining ? 5 : (generationCount-(candidate == 1 ? loserPityGeneration1 : loserPityGeneration2))/75) && generationCount-(candidate == 1 ? loserPityGeneration1 : loserPityGeneration2) > 100))
        {
            if (candidate == 1) loserPityGeneration1 = generationCount;
            else loserPityGeneration2 = generationCount;
            GenerateCandidates(candidates);
            return;
        }
        int index = oldCandidates.Count - 1;
        int children = 0;
        int mate = oldCandidates.Count - 2;
        while(candidates.Count < startPopulation/2)
        {
            if(children > fitness[index] / 10)
            {
                children = 0;
                index--;
                if (index < 0) index = oldCandidates.Count - 1;
                mate = index - 1;
                if (mate < 0) mate = oldCandidates.Count - 1;
            }
            float mutation = Random.Range(0f, 1f);
            if(mutation < mutationChance)
            {
                candidates.Add(new Candidate(oldCandidates[index], oldCandidates[mate], mutationValue));
            }
            else
            {
                candidates.Add(new Candidate(oldCandidates[index], oldCandidates[mate]));
            }
            children++;
            mate--;
            if (mate < 0) mate = oldCandidates.Count - 1;
        }
    }

    private void GetAverageWeights()
    {
        int currentAverageWeightsTotal = 0;
        int previousAverageWeightsTotal = 0;
        if (turnsRemembered <= 1 || candidateType == CandidateType.Basic)
        {
            for (int i = 0; i < (pairTraining ? averageWeights.Length : averageWeights.Length/2); i++)
            {
                previousAverageWeightsTotal += int.Parse(averageWeights[i].GetComponent<Text>().text);
                List<Candidate> candidates;
                candidates = i < averageWeights.Length / 2 ? candidatesP1 : candidatesP2;
                int total = 0;
                for (int j = 0; j < startPopulation / 2; j++)
                {
                    total += candidates[j].GetWeight(i % (averageWeights.Length / 2));
                }
                total /= startPopulation / 2;
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
            allAverageChanges.Add(Mathf.Abs(currentAverageWeightsTotal - previousAverageWeightsTotal));
            changeNumber.GetComponent<Text>().text = totalAverageChanges[0].ToString();
        }
        else
        {
            previousAverageWeightsTotal = lastTotalWeights;
            for (int i = 0; i < (pairTraining ? startPopulation : startPopulation / 2); i++)
            {
                currentAverageWeightsTotal += i < startPopulation / 2 ? candidatesP1[i].GetWeights().AsParallel().Sum() : candidatesP2[i-(startPopulation/2)].GetWeights().AsParallel().Sum();
            }
            totalAverageChanges.Add(Mathf.Abs(currentAverageWeightsTotal - previousAverageWeightsTotal));
            allAverageChanges.Add(Mathf.Abs(currentAverageWeightsTotal - previousAverageWeightsTotal));
            lastTotalWeights = currentAverageWeightsTotal;
            for(int i = 0; i < totalAverageChanges.Count; i++)
            {
                averageWeights[i].GetComponent<Text>().color = new Color(totalAverageChanges[i] > 5000 ? (float)totalAverageChanges[i]/100000 : 0, 
                    totalAverageChanges[i] <= 5000 ? (float)(5000 - totalAverageChanges[i])/5000 : 0, 0);
                averageWeights[i].GetComponent<Text>().text = totalAverageChanges[i].ToString();
            }
        }
        
        int averageLastFive = 0;
        foreach(int i in totalAverageChanges)
        {
            averageLastFive += i;
        }
        if (averageLastFive <= mutationValue && generationCount > 500 && (pairTraining || survivorCount1 >= 49) && (unit1.GetUnitName() == unit2.GetUnitName() || (survivorCount1 > 15 && survivorCount2 > 15)))
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
                    if (pairTraining) set.transform.GetChild(1).name = candidatesP2[i++].GetWeightsAsString();
                    else set.transform.GetChild(1).name = "Best";
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
            if (unit == 1 && !pairTraining) return candidates[0].GetCandidateAction(u, enemy, th);
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
                Reproduce(candidatesP1, 1);
                if (pairTraining) Reproduce(candidatesP2, 2);
                if(pairTraining) fitness = new List<float>(new float[startPopulation]);
                else fitness = new List<float>(new float[startPopulation/2]);
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
        int bestPlayer = 0;
        if(survivorCount1 > survivorCount2)
        {
            bestPlayer = 0;
        }
        else
        {
            bestPlayer = 1;
        }
        if (candidateType == CandidateType.Basic)
        {
            if (bestPlayer == 0)
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
            }
            else
            {
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
        }
        else
        {
            Debug.Log("Done");
            if (turnsRemembered > 1) {
                List<Candidate> candidates = bestPlayer == 0 ? candidatesP1 : candidatesP2;
                int offset = bestPlayer == 0 ? 0 : lastAverageWeights.Length / 2;
                for (int i = 0; i < lastAverageWeights.Length/2; i++)
                {
                    int total = 0;
                    for (int j = 0; j < startPopulation / 2; j++)
                    {
                        total += candidates[j].GetWeight(i % (lastAverageWeights.Length / 2));
                    }
                    total /= startPopulation / 2;
                    lastAverageWeights[i+offset] = total;
                }
            }
        }
        string path1;
        string path2;
        string complexity = "";
        if (candidateType == CandidateType.Basic) complexity = "Basic";
        else if (candidateType == CandidateType.Advanced) complexity = "Advanced";
        else if (candidateType == CandidateType.AdvancedTrainingPartner) complexity = "Advanced+";
        else if (candidateType == CandidateType.AdvancedHeuristic) complexity = "AdvancedHeuristic";
        if (unit1.GetUnitName() == unit2.GetUnitName())
        {
            path1 = Application.dataPath + "/Best Candidates/" + unit1.GetUnitName() + "-" + unit2.GetUnitName() + "-" + complexity + "-" + count;
            path2 = "";
        }
        else
        {
            path1 = Application.dataPath + "/Best Candidates/" + unit1.GetUnitName() + "-" + unit2.GetUnitName() + "-" + complexity + "-" + count;
            path2 = Application.dataPath + "/Best Candidates/" + unit2.GetUnitName() + "-" + unit1.GetUnitName() + "-" + complexity + "-" + count;
        }
        if (candidateType == CandidateType.Advanced || candidateType == CandidateType.AdvancedTrainingPartner || candidateType == CandidateType.AdvancedHeuristic) { path1 += "-" + turnsRemembered; path2 += "-" + turnsRemembered; }
        if (path2.Length < 10) File.WriteAllText(path1, GetBestWeightsAsString(bestPlayer));
        else
        {
            File.WriteAllText(path1, GetBestWeightsAsString(0));
            if (pairTraining) File.WriteAllText(path2, GetBestWeightsAsString(1));
        }
        float time = Time.time - lastTime;
        lastTime = Time.time;
        Debug.Log("Time taken: " + time);
        string path = Application.dataPath + "/Results/" + unit1.GetUnitName() + "-" + unit2.GetUnitName() + "-" + complexity;
        if (File.Exists(path))
        {
            File.AppendAllText(path, "Generations: " + generationCount + " Time Taken: " + time + " " + turnsRemembered + "\n");
        }
        else
        {
            path = Application.dataPath + "/Results/" + unit2.GetUnitName() + "-" + unit1.GetUnitName() + "-" + complexity;
            if (File.Exists(path))
            {
                File.AppendAllText(path, "Generations: " + generationCount + " Time Taken: " + time + " " + turnsRemembered + "\n");
            }
            else
            {
                File.WriteAllText(path, "Generations: " + generationCount + " Time Taken: " + time + " " + turnsRemembered + "\n");
            }
        }
        path = Application.dataPath + "/Graphs/" + unit1.GetUnitName() + "-" + unit2.GetUnitName() + "-" + complexity + ".csv";
        File.WriteAllText(path, "" + allAverageChanges[1]);
        for(int i = 2; i < allAverageChanges.Count; i++)
        {
            File.AppendAllText(path, "\n" + allAverageChanges[i]);
        }
        RestartEvolution();
    }

    private void RestartEvolution()
    {
        count++;
        if(count <= endValue)
        {
            candidatesP1.Clear();
            if (pairTraining) candidatesP2.Clear();
            allAverageChanges.Clear();
            totalAverageChanges.Clear();
            initialAverageWeights = new int[geneLength * 2];
            lastAverageWeights = new int[geneLength * 2];
            GenerateCandidates(candidatesP1);
            if (pairTraining) GenerateCandidates(candidatesP2);
            generationCount = 1;
            for(int i = 0; i < averageWeights.Length; i++) 
            {
                averageWeights[i].GetComponent<Text>().text = "0";
            }
            bestFound = false;
        }
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
}
