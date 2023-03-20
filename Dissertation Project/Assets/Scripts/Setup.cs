using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    public GameObject carrier;

    [SerializeField]
    private Controller controller;

    [SerializeField]
    private GameObject player1;

    [SerializeField]
    private GameObject player2;
    // Start is called before the first frame update
    void Start()
    {
        carrier = GameObject.Find("Carrier");
        if (carrier == null) Destroy(gameObject);
        else
        {
            Carrier c = carrier.GetComponent<Carrier>();
            c.player1.transform.parent = player1.transform;
            player1.GetComponent<UnitGraphics>().SetUnit(c.player1.GetComponent<Unit>());
            c.player2.transform.parent = player2.transform;
            player2.GetComponent<UnitGraphics>().SetUnit(c.player2.GetComponent<Unit>());
            controller.SetController(c.player1.GetComponent<Unit>(), c.player2.GetComponent<Unit>(), c.ct1, c.ct2, GameObject.Find("TurnHandler").GetComponent<TurnHandler>(), false, 3);
            controller.gameObject.SetActive(true);
            player1.SetActive(true);
            player2.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
