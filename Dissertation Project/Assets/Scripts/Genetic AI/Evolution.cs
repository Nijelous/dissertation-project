using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Evolution : MonoBehaviour
{
    private List<Candidate> candidates;
    private List<float> fitness;
    [SerializeField]
    private int startPopulation = 100;
    [SerializeField]
    private bool showEvolution;
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
    // Start is called before the first frame update
    void Start()
    {
        candidates = new List<Candidate>();
        sets = new List<GameObject>();
        evaluatingSets = new List<GameObject>();
        fitness = new List<float>(new float[startPopulation]);
        GenerateCandidates();
        if (showEvolution)
        {
            playerSelect.SetActive(false);
            player1.GetComponent<SpriteRenderer>().enabled = false;
            player2.GetComponent<SpriteRenderer>().enabled = false;
            for(int i = 0; i < startPopulation/2; i++)
            {

                GameObject parent = Instantiate(setPrefab, canvas.transform);
                parent.name = "Set " + i;
                parent.transform.localPosition = new Vector3(0, 0, 0);
                sets.Add(parent);
                GameObject bar1 = Instantiate(player1Bars);
                bar1.transform.SetParent(parent.transform);
                bar1.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1);
                bar1.GetComponent<RectTransform>().localPosition = new Vector3(-525 + 228 * (i % 5), 200 - 45 * (i / 5), 0);
                GameObject bar2 = Instantiate(player1Bars);
                bar2.transform.SetParent(parent.transform);
                bar2.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1);
                bar2.GetComponent<RectTransform>().localPosition = new Vector3(-415 + 228 * (i % 5), 200 - 45 * (i / 5), 0);
                GameObject vs = Instantiate(roundNumber);
                vs.transform.SetParent(parent.transform);
                vs.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.4f, 1);
                vs.GetComponent<RectTransform>().localPosition = new Vector3(-470 + 228 * (i % 5), 180 - 45 * (i / 5), 0);
                vs.GetComponent<Text>().text = "VS";
            }
            roundNumber.transform.localPosition = new Vector3(123, 234, 0);
            generationNumber = Instantiate(roundNumber, canvas.transform);
            generationNumber.transform.localPosition = new Vector3(-123, 234, 0);
            generationNumber.GetComponent<Text>().text = "Generation " + generationCount;
            player1Bars.SetActive(false);
            player2Bars.SetActive(false);
            averageWeights = new GameObject[] {Instantiate(roundNumber, canvas.transform),  Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform),
                Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform), Instantiate(roundNumber, canvas.transform), 
                Instantiate(roundNumber, canvas.transform) };
            initialAverageWeights = new int[7];
            for(int i = 0; i < averageWeights.Length; i++)
            {
                averageWeights[i].name = "AverageWeights " + i;
                averageWeights[i].GetComponent<RectTransform>().sizeDelta = new Vector2(36, 36);
                averageWeights[i].GetComponent<RectTransform>().localPosition = new Vector3(236 + (i*40), 234, 0);
                averageWeights[i].GetComponent<Text>().text = "0";
            }
            StartCoroutine(LateStart());
        }
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        foreach (GameObject set in sets)
        {
            Unit u1 = set.transform.GetChild(0).gameObject.AddComponent<Unit>();
            u1.SetAttributes(unit1);
            set.transform.GetChild(0).gameObject.AddComponent<UnitGraphics>();
            Unit u2 = set.transform.GetChild(1).gameObject.AddComponent<Unit>();
            u2.SetAttributes(unit2);
            set.transform.GetChild(1).gameObject.AddComponent<UnitGraphics>();
            set.GetComponent<Controller>().SetController(u1, u2, ControlType.GeneticPair, ControlType.GeneticPair, set.GetComponent<TurnHandler>(), quickCombat, waitTime);
        }
        for (int i = 0; i < 7; i++)
        {
            GameObject display = Instantiate(roundNumber, canvas.transform);
            display.name = "ActionDisplay " + i;
            display.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 20);
            display.GetComponent<RectTransform>().localPosition = new Vector3(-450 + (i * 150), -254, 0);
            display.GetComponent<Text>().fontSize = 18;
            display.GetComponent<Text>().text = i > 2 ?
                unit1.GetMagicalActions().ToArray()[i - 3].GetActionName() : unit1.GetPhysicalActions().ToArray()[i].GetActionName();
        }
        started = true;
    }

    private void GenerateCandidates()
    {
        for(int i = 0; i < startPopulation; i++)
        {
            candidates.Add(new Candidate());
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
                fitness[i * 2] = 0;
                fitness[(i * 2) + 1] = 100;
            }
            else if(u2.GetHealth() == 0) {
                fitness[i * 2] = 0;
                fitness[(i * 2) + 1] = 100;
            }
            else
            {
                fitness[i * 2] = u1.GetHealth() - u2.GetHealth();
                fitness[(i * 2) + 1] = u2.GetHealth() - u1.GetHealth();
            }
        }
    }

    private void SortPopulation()
    {
        for(int i = 0; i < candidates.Count; i++)
        {
            if (fitness[i] == 0)
            {
                candidates.RemoveAt(i);
                fitness.RemoveAt(i);
                i--;
            }
        }
        QuickSort(0, fitness.Count-1);
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
                (candidates[right], candidates[left]) = (candidates[left], candidates[right]);
            }
            else return right;
        }

    }

    private void Reproduce()
    {
        List<Candidate> oldCandidates = new List<Candidate>(candidates);
        candidates.Clear();
        if(oldCandidates.Count == 0)
        {
            GenerateCandidates();
            return;
        }
        int reverseCount = oldCandidates.Count-1;
        for(int i = 0; i < startPopulation; i++)
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
        for(int i = 0; i < 7; i++)
        {
            int total = 0;
            for(int j = 0; j < startPopulation; j++)
            {
                total += candidates[j].GetActionWeight(i);
            }
            total /= startPopulation;
            if (int.Parse(averageWeights[i].GetComponent<Text>().text) == 0) initialAverageWeights[i] = total;
            else
            {
                averageWeights[i].GetComponent<Text>().color = new Color
                    (total < initialAverageWeights[i] ? (float)(initialAverageWeights[i] - total) / initialAverageWeights[i] : 0,
                    total > initialAverageWeights[i] ? (float)(total - initialAverageWeights[i]) / (99 - initialAverageWeights[i]) : 0, 0);
                averageWeights[i].GetComponent<Text>().text = total.ToString();
            }
            averageWeights[i].GetComponent<Text>().text = total.ToString();
        }
    }

    private void Evaluate()
    {
        int i = 0;
        foreach(GameObject set in sets)
        {
            evaluatingSets.Add(set);
            set.transform.GetChild(0).GetChild(4).GetComponent<Text>().text = candidates[i++].GetWeightsAsString();
            set.transform.GetChild(1).GetChild(4).GetComponent<Text>().text = candidates[i++].GetWeightsAsString();
            set.GetComponent<Controller>().ResetBattle();
            set.GetComponent<Controller>().Evaluate();
            generationFinished = false;
        }
    }

    public int GetCandidateAction(int set, int unit, Unit u)
    {
        if(set < 50 && unit < 2)
        {
            int candidate = set + unit;
            int currentAction = 0;
            int currentActionWeight = 0;
            for(int i = 0; i < 7; i++)
            {
                if(i > 2)
                {
                    if (u.GetMana() < u.GetMagicalActions().ToArray()[i-3].GetManaCost())
                    {
                        continue;
                    }
                }
                if (candidates[candidate].GetActionWeight(i) > currentActionWeight)
                {
                    currentAction = i;
                    currentActionWeight = candidates[candidate].GetActionWeight(i);
                }
            }
            return currentAction;
        }
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            if (evaluatingSets.Count > 0)
            {
                roundNumber.GetComponent<Text>().text = "Round " + evaluatingSets[0].GetComponent<Controller>().GetRound();
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
                Evaluate();
            }
            else
            {
                FindFitness();
                SortPopulation();
                Reproduce();
                fitness = new List<float>(new float[startPopulation]);
                generationCount++;
                generationNumber.GetComponent<Text>().text = "Generation " + generationCount;
                generationFinished = true;
            }
        }
    }
}
