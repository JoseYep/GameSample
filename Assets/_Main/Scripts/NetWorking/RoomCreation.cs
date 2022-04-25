using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class RoomCreation : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private int maxPlayers = 2;
    private int roomIndex = 0;
    private string lastRoomName;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        lastRoomName = "Room";
        playButton.onClick.AddListener(() => JoinOrCreateRoom(lastRoomName));
    }

    public void JoinOrCreateRoom(string roomName)
    {
        lastRoomName = roomName;
        string room = (lastRoomName + " " + roomIndex).ToString();
        PhotonNetwork.JoinOrCreateRoom(room, new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = (byte)maxPlayers, BroadcastPropsChangeToAll = true }, null);
        statusText.text = "Searching opponent...";
        Debug.Log("Loggin on room: " + room);
    }

    public override void OnLeftRoom()
    {
        roomIndex++;
        JoinOrCreateRoom(lastRoomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        roomIndex++;
        JoinOrCreateRoom(lastRoomName);
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        while (true)
        {
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
}
