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

    [Header("Smart Contracts")]
    [SerializeField] private GameSmartContract smartContract;

    [Header("UI")]
    [SerializeField] private Button rollButton;
    [SerializeField] private UIEndScreen endScene;

    private PlayerNetworkAgent playerNetworkAgent;
    private PlayerNetworkAgent opponentNetworkAgent;

    private GameObject localPlayerDice;
    private GameObject localOpponentDice;

    private bool playersSet = false;
    private int maxRounds = 3;
    private bool localWinner = false;
    private bool gameOver = false;
    private int playersReady = 0;
    private int actualRound = 0;
    private int playerRoundsWinned = 0;
    private int opponentRoundsWinned = 0;

    public bool PlayersSet { get => playersSet; set => playersSet = value; }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                SpawnPlayer();
                break;
            }

            yield return null;
        }     
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

            //Notify contract i win
            endScene.Loser = opponentNetworkAgent.Account;
            endScene.Winner = playerNetworkAgent.Account;
            endScene.EndMatch(true);

            //Debug.Log("Winner wallet: " + playerNetworkAgent.Account);
            //Debug.Log("Loser wallet: " + opponentNetworkAgent.Account);

            //smartContract.Claim(playerNetworkAgent.Account, opponentNetworkAgent.Account, 0);
        }
        else
        {
            resultText.text = "YOU LOSE";
            //Notify contract i lose
            endScene.EndMatch(false);

        }

        yield return new WaitForSeconds(gameFinishDelay);
        
        //if (SceneChanger.instance)
        //{
        //    SceneChanger.instance.LeaveAndCahngeScene(0);
        //}
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
            //Clean opponent 
            if (localOpponentDice)
            {
                Destroy(localOpponentDice);
                opponentNetworkAgent = null;
            }

            localOpponentDice = Instantiate(dicePrefab, opponentSpawn.position, Quaternion.identity);
            localOpponentDice.name = "OpponentDice";
            obj.name = "OpponentAgent";
            opponentNetworkAgent = agent;
        }

        //Checkt if players created
        if (localOpponentDice && localPlayerDice)
        {
            PlayersSet = true;
        }
        else
        {
            PlayersSet = false;
        }

        Debug.Log("Local player character created");
    }
}
