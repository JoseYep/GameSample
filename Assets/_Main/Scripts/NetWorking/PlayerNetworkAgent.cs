using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerNetworkAgent : MonoBehaviourPun
{
    [Header("Game Events")]
    [SerializeField] private GameEvent onPlayerJoin;
    [SerializeField] private GameEvent onPlayerReady;
    [SerializeField] private GameEvent onPlayerDiceResult;

    private Button rollButton;

    private string account;
    private int diceNumber = 0; // 1 to 6
    private PhotonView view;
    private DiceGameManager manager;

    #region Properties
    public PhotonView View { get => view; set => view = value; }
    public int DiceNumber { get => diceNumber; set => diceNumber = value; }
    public string Account { get => account; set => account = value; }
    #endregion

    private void Awake()
    {
        Account = PlayerPrefs.GetString("Account");
        manager = GameObject.FindObjectOfType<DiceGameManager>();

        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            PlayerJoined();
            rollButton = GameObject.FindGameObjectWithTag("RollButton").GetComponent<Button>();
            if (rollButton)
            {
                rollButton.onClick.AddListener(() => SetNumber());
            }
        }
    }
    #region Player Methods
    public void SetNumber()
    {
        int randomNumber = Random.Range(1, 6);
        rollButton.interactable = false;
        view.RPC("SetDiceNumber", RpcTarget.All, randomNumber);
    }

    public void SaveAdress()
    {
        view.RPC("SavePlayerWallet", RpcTarget.All, account);
    }

    public void PlayerJoined()
    {
        view.RPC("PlayerJoinGame", RpcTarget.All);
        SaveAdress();
    }
    #endregion

    #region Photon RPCs
    [PunRPC] 
    private void SetDiceNumber(int number)
    {
        this.diceNumber = number;
        onPlayerDiceResult.Raise(number);
        onPlayerReady.Raise(gameObject);
    }

    [PunRPC]
    private void PlayerJoinGame()
    {
        onPlayerJoin.Raise(this.gameObject);
    }

    [PunRPC]
    private void SavePlayerWallet(string adress)
    {
        this.account = adress;
    }
    #endregion
}
