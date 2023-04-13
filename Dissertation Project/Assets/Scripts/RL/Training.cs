using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private GameObject set;
    private int setNumber;
    private GameObject explorationRateGO;
    private int explorationRate;
    private GameObject winLossOverall;
    private int[] results;
    private GameObject winLossLast100;
    private List<int> last100;

    private Candidate partner;
    private QActor actor;
    // Start is called before the first frame update
    void Start()
    {
        explorationRate = 1;
        setNumber = 1;
        results = new int[2];
        last100 = new();
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
        set.GetComponent<Text>().text = "Set 1";
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
