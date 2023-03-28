using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public ControlType ct1;

    public ControlType ct2;

    public GameObject player1;

    public GameObject player2;

    public GameObject fighter;

    public GameObject rogue;

    public GameObject wizard;

    public GameObject classes;

    public GameObject players;

    public GameObject aiType;

    public GameObject geneticType;

    public GameObject geneticTurnsRemembered;

    public Text prompt;

    public GameObject carrier;

    private int playerNumber;

    private int[] turnsRemembered;

    private int playerSelecting = 1;

    // Start is called before the first frame update
    void Start()
    {
        turnsRemembered = new int[2];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ZeroPlayers()
    {
        playerNumber = 0;
        classes.SetActive(true);
        prompt.text = "Select Player 1 Class";
        players.SetActive(false);
    }

    public void OnePlayer()
    {
        playerNumber = 1;
        classes.SetActive(true);
        prompt.text = "Select Player 1 Class";
        players.SetActive(false);
    }

    public void TwoPlayers()
    {
        playerNumber = 2;
        classes.SetActive(true);
        prompt.text = "Select Player 1 Class";
        players.SetActive(false);
    }

    public void Fighter()
    {
        if(playerSelecting == 1)
        {
            player1 = Instantiate(fighter);
        }
        else
        {
            player2 = Instantiate(fighter);
        }
        if(playerSelecting > playerNumber)
        {
            aiType.SetActive(true);
            prompt.text = "Select Player " + playerSelecting + " AI";
            classes.SetActive(false);
        }
        else
        {
            if (playerSelecting == 1) ct1 = ControlType.Human;
            else ct2 = ControlType.Human;
            playerSelecting++;
            if(playerSelecting == 3)
            {
                Game();
            }
            prompt.text = "Select Player 2 Class";
        }
    }

    public void Rogue()
    {
        if (playerSelecting == 1)
        {
            player1 = Instantiate(rogue);
        }
        else
        {
            player2 = Instantiate(rogue);
        }
        if (playerSelecting > playerNumber)
        {
            aiType.SetActive(true);
            prompt.text = "Select Player " + playerSelecting + " AI";
            classes.SetActive(false);
        }
        else
        {
            if (playerSelecting == 1) ct1 = ControlType.Human;
            else ct2 = ControlType.Human;
            playerSelecting++;
            if (playerSelecting == 3)
            {
                Game();
            }
            prompt.text = "Select Player 2 Class";
        }
    }

    public void Wizard()
    {
        if (playerSelecting == 1)
        {
            player1 = Instantiate(wizard);
        }
        else
        {
            player2 = Instantiate(wizard);
        }
        if (playerSelecting > playerNumber)
        {
            aiType.SetActive(true);
            prompt.text = "Select Player " + playerSelecting + " AI";
            classes.SetActive(false);
        }
        else
        {
            if (playerSelecting == 1) ct1 = ControlType.Human;
            else ct2 = ControlType.Human;
            playerSelecting++;
            if (playerSelecting == 3)
            {
                Game();
            }
            prompt.text = "Select Player 2 Class";
        }
    }

    public void Random()
    {
        if(playerSelecting == 1)
        {
            ct1 = ControlType.Random;
            playerSelecting++;
            classes.SetActive(true);
            prompt.text = "Select Player 2 Class";
            Debug.Log(player1.GetComponent<Unit>().GetPhysicalActions().ToArray()[0]);
            aiType.SetActive(false);
        }
        else
        {
            ct2 = ControlType.Random;
            Game();
        }
    }

    public void Genetic()
    {
        geneticType.SetActive(true);
        prompt.text = "Select Genetic Type";
        aiType.SetActive(false);
    }

    public void GeneticBasic()
    {
        if (playerSelecting == 1)
        {
            ct1 = ControlType.GeneticSoloBasic;
            playerSelecting++;
            classes.SetActive(true);
            prompt.text = "Select Player 2 Class";
            geneticType.SetActive(false);
        }
        else
        {
            ct2 = ControlType.GeneticSoloBasic;
            Game();
        }
    }

    public void GeneticAdvanced()
    {
        if (playerSelecting == 1)
        {
            ct1 = ControlType.GeneticSoloAdvanced;
            geneticTurnsRemembered.SetActive(true);
            for(int i = 0; i < 3; i++)
            {
                geneticTurnsRemembered.transform.GetChild(i).gameObject.SetActive(true);
            }
            prompt.text = "Select How Many Turns You Want the AI to Remember";
            geneticType.SetActive(false);
        }
        else
        {
            ct2 = ControlType.GeneticSoloAdvanced;
            geneticTurnsRemembered.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                geneticTurnsRemembered.transform.GetChild(i).gameObject.SetActive(true);
            }
            prompt.text = "Select How Many Turns You Want the AI to Remember";
            geneticType.SetActive(false);
        }
    }

    public void GeneticAdvancedHeuristic()
    {
        if (playerSelecting == 1)
        {
            ct1 = ControlType.GeneticSoloAdvanced;
            geneticTurnsRemembered.SetActive(true);
            for (int i = 0; i < 4; i++)
            {
                geneticTurnsRemembered.transform.GetChild(i).gameObject.SetActive(true);
            }
            prompt.text = "Select How Many Turns You Want the AI to Remember";
            geneticType.SetActive(false);
        }
        else
        {
            ct2 = ControlType.GeneticSoloAdvanced;
            geneticTurnsRemembered.SetActive(true);
            for (int i = 0; i < geneticTurnsRemembered.transform.childCount; i++)
            {
                geneticTurnsRemembered.transform.GetChild(i).gameObject.SetActive(true);
            }
            prompt.text = "Select How Many Turns You Want the AI to Remember";
            geneticType.SetActive(false);
        }
    }

    public void SetTurnsRemembered(int turns)
    {
        
        if(playerSelecting == 1)
        {
            turnsRemembered[0] = turns;
            playerSelecting++;
            classes.SetActive(true);
            for (int i = 0; i < geneticTurnsRemembered.transform.childCount; i++)
            {
                geneticTurnsRemembered.transform.GetChild(i).gameObject.SetActive(false);
            }
            prompt.text = "Select Player 2 Class";
            geneticTurnsRemembered.SetActive(false);

        }
        else
        {
            turnsRemembered[1] = turns;
            Game();
        }
    }

    private void Game()
    {
        carrier.GetComponent<Carrier>().ct1 = ct1;
        carrier.GetComponent<Carrier>().ct2 = ct2;
        carrier.GetComponent<Carrier>().turnsRemembered = turnsRemembered;
        DontDestroyOnLoad(player1);
        player1.name = player1.GetComponent<Unit>().GetUnitName() + " 1";
        carrier.GetComponent<Carrier>().player1 = player1;
        DontDestroyOnLoad(player2);
        player2.name = player2.GetComponent<Unit>().GetUnitName() + " 2";
        carrier.GetComponent<Carrier>().player2 = player2;
        DontDestroyOnLoad(carrier);
        SceneManager.LoadScene(1);
    }
}
