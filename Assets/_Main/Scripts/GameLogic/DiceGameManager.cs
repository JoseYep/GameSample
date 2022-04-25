using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class DiceGameManager : MonoBehaviourPunCallbacks
{  
    [Header("Game Settings")]
    [SerializeField] private float rollTime = 12;
    [SerializeField] private float gameFinishDelay = 5f;

    [Header("Utilities")]
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform opponentSpawn;
    [SerializeField] private UIRoundHandler roundHandler;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("UI")]
    [SerializeField] private Button rollButton;

    private PlayerNetworkAgent playerNetworkAgent;
    private PlayerNetworkAgent opponentNetworkAgent;

    private GameObject localPlayerDice;
    private GameObject localOpponentDice;
    
    private int maxRounds = 3;
    private bool localWinner = false;
    private bool gameOver = false;
    private int playersReady = 0;
    private int actualRound = 0;
    private int playerRoundsWinned = 0;
    private int opponentRoundsWinned = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject netWorkingPlayer = PhotonNetwork.Instantiate("PlayerNetworkAgent", Vector3.zero, Quaternion.identity);        
    }

    private IEnumerator StartGame()
    {      
        localPlayerDice.GetComponent<DiceController>().Roll(playerNetworkAgent.DiceNumber,rollTime);
        localOpponentDice.GetComponent<DiceController>().Roll(opponentNetworkAgent.DiceNumber, rollTime);
        yield return new WaitForSeconds(rollTime);
        RoundResult();       
    }

    private void RoundResult()
    {
        if (!gameOver)
        {
            if (playerNetworkAgent.DiceNumber > opponentNetworkAgent.DiceNumber)
            {
                roundHandler.SetResult(actualRound, true);
                actualRound++;
                playerRoundsWinned++;
                resultText.text = "ROUND WIN";
                rollButton.interactable = true;

                if (playerRoundsWinned > 1)
                {
                    gameOver = true;
                    localWinner = true;
                    StartCoroutine(GameOver());
                }
            }
            else if (playerNetworkAgent.DiceNumber < opponentNetworkAgent.DiceNumber)
            {
                roundHandler.SetResult(actualRound, false);
                opponentRoundsWinned++;
                actualRound++;
                resultText.text = "ROUND LOSE";
                rollButton.interactable = true;

                if (opponentRoundsWinned > 1)
                {
                    gameOver = true;
                    localWinner = false;
                    StartCoroutine(GameOver());
                }

            }
            else
            {
                resultText.text = "TIE";
                rollButton.interactable = true;
            }

            playersReady = 0;
        }
    }

    private IEnumerator GameOver()
    {
        rollButton.interactable = false;

        if (localWinner)
        {
            resultText.text = "YOU WIN";
            //Notify contract i loose
        }
        else
        {
            resultText.text = "YOU LOOSE";
            //Notify contract i win
        }
        yield return new WaitForSeconds(gameFinishDelay);
        
        if (SceneChanger.instance)
        {
            SceneChanger.instance.LeaveAndCahngeScene(0);
        }
    }

    public void PlayerReady(GameObject obj)
    {
        playersReady++;

        if (playersReady == 2)
        {
            rollButton.interactable = false;
            StartCoroutine(StartGame());
        }
    }

    public void GetPlayer(GameObject obj)
    {
        PlayerNetworkAgent agent = obj.GetComponent<PlayerNetworkAgent>();
        if (agent.photonView.IsMine)
        {
            localPlayerDice = Instantiate(dicePrefab, playerSpawn.position, Quaternion.identity);
            localPlayerDice.name = "PlayerDice";
            obj.name = "PlayerAgent";
            playerNetworkAgent = agent;
        }
        else 
        {
            localOpponentDice = Instantiate(dicePrefab, opponentSpawn.position, Quaternion.identity);
            localOpponentDice.name = "OpponentDice";
            obj.name = "OpponentAgent";
            opponentNetworkAgent = agent;
        }

        Debug.Log("Local player character created");
    }
}
