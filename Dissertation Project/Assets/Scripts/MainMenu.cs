using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

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

    public Text prompt;

    public GameObject carrier;

    private int playerNumber;

    private int playerSelecting = 1;

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
            DontDestroyOnLoad(player1);
        }
        else
        {
            player2 = Instantiate(fighter);
            DontDestroyOnLoad(player2);
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
            DontDestroyOnLoad(player1);
        }
        else
        {
            player2 = Instantiate(rogue);
            DontDestroyOnLoad(player2);
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
            DontDestroyOnLoad(player1);
        }
        else
        {
            player2 = Instantiate(wizard);
            DontDestroyOnLoad(player2);
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
        if (playerSelecting == 1)
        {
            ct1 = ControlType.GeneticSolo;
            playerSelecting++;
            classes.SetActive(true);
            prompt.text = "Select Player 2 Class";
            aiType.SetActive(false);
        }
        else
        {
            ct2 = ControlType.GeneticSolo;
            Game();
        }
    }

    private void Game()
    {
        carrier.GetComponent<Carrier>().ct1 = ct1;
        carrier.GetComponent<Carrier>().ct2 = ct2;
        carrier.GetComponent<Carrier>().player1 = player1;
        carrier.GetComponent<Carrier>().player2 = player2;
        DontDestroyOnLoad(carrier);
        SceneManager.LoadScene(1);
    }
}
