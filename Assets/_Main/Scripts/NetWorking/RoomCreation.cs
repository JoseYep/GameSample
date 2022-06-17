using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System;

public class RoomCreation : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameSmartContract smartContract;
    [SerializeField] private long bet = 10000000000;

    private int maxPlayers = 2;
    private int roomIndex = 0;
    private string lastRoomName;

    private RoomNetworkAgent localPlayer;
    private RoomNetworkAgent opponentPlayer;
    private bool host = false;

    private int playersReady = 0;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        lastRoomName = "Room";

        smartContract.onFailed += Restart;
        
        playButton.onClick.AddListener(() => JoinOrCreateRoom(lastRoomName));
        playButton.onClick.AddListener(() => playButton.interactable = false);
    }

    private void SpawnPlayer()
    {
        GameObject netWorkingPlayer = PhotonNetwork.Instantiate("RoomAgent", Vector3.zero, Quaternion.identity);
    } 

    public void JoinOrCreateRoom(string roomName)
    {
        smartContract.GetGame();
        lastRoomName = roomName;
        string room = (lastRoomName + " " + roomIndex).ToString();
        PhotonNetwork.JoinOrCreateRoom(room, new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = (byte)maxPlayers, BroadcastPropsChangeToAll = true }, null);
        statusText.text = "Searching for players...";

        Debug.Log("Loggin on room: " + room);
    }

    public override void OnLeftRoom()
    {
        roomIndex++;
        JoinOrCreateRoom(lastRoomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room non available, searching for a new one");
        roomIndex++;
        JoinOrCreateRoom(lastRoomName);
    }

    public override void OnCreatedRoom()
    {      
        host = true;

        StartCoroutine(InitPlayer(host));

        Debug.Log("Game created");
    }
    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            host = false;

            StartCoroutine(InitPlayer(host));

            Debug.Log("Game Joined");          
        }

        StartCoroutine(ValidateForStart());
    }

    private IEnumerator InitPlayer(bool isHost)
    {
        SpawnPlayer();
        yield return new WaitUntil(() => localPlayer != null);
        smartContract.onComplete += localPlayer.PaymentComplete;
        
        if (host)
        {
            StartCoroutine(DisplayPayment());
        }

        Debug.Log("Player initializated!");
    }

    private IEnumerator StartGame()
    {    
        while (true)
        {
            statusText.text = "Searching opponent...";

            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                statusText.text = "Opponent found";

                if (SceneChanger.instance)
                {
                    SceneChanger.instance.ChangeScene(1);
                }

                break;
            }

            yield return null;
        }
    }
    
    private IEnumerator DisplayPayment()
    {
        while (true)
        {          
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                if (host)
                {
                    smartContract.CreateGame(bet);
                }             
                break;
            }

            yield return null;
        }
    }

    private void DisplayGuestPayment()
    {
        smartContract.JoinGame(bet);
    }

    private IEnumerator ValidateForStart()
    {
        while (true)
        {
            statusText.text = "Preparing match, please wait...";

            if (playersReady >= 2)
            {
                statusText.text = "Starting game...";

                if (SceneChanger.instance)
                {
                    SceneChanger.instance.ChangeScene(1);
                }

                break;
            }
            
            yield return new WaitForEndOfFrame();
        }

    }

    public void PlayersPayed()
    {
        playersReady++;
        Debug.Log("Players ready: " + playersReady);

        if (playersReady < 2)
        {
            if (!host)
            {
                Debug.Log("Host ready to play");
                DisplayGuestPayment();
            }
            else
            {
                Debug.Log("Waiting for guest to pay");
                smartContract.onComplete -= localPlayer.PaymentComplete;
            }
        }     
    }

    private void StartGameScene()
    {
        smartContract.onFailed -= Restart;

        if (SceneChanger.instance)
        {
            SceneChanger.instance.ChangeScene(1);
        }
    }

    private void Restart()
    {
        smartContract.onFailed -= Restart;

        if (SceneChanger.instance)
        {
            SceneChanger.instance.LeaveAndCahngeScene(0);
        }
    }

    public void GetPlayer(GameObject obj)
    {
        RoomNetworkAgent agent = obj.GetComponent<RoomNetworkAgent>();
       
        if (agent.photonView.IsMine)
        {        
            localPlayer = agent;
            Debug.Log("Local player found");
        }
        else
        {
            opponentPlayer = agent;
            Debug.Log("Opponet found");
        }
    }
}
