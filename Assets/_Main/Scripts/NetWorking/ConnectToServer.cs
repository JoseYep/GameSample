using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button playButton;
    [SerializeField] private string gameVersion = "1";

    private IEnumerator Start()
    {
        playButton.interactable = false;
        PhotonNetwork.AutomaticallySyncScene = true;

        yield return new WaitForEndOfFrame();

        ServerSettings.ResetBestRegionCodeInPreferences();

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            StartCoroutine(TryToConnect());
        }
        else
        {
            playButton.interactable = true;
        }
    }

    private IEnumerator TryToConnect()
    {
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Connecting to Network...");
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
            yield return new WaitForEndOfFrame();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinLobby();
            playButton.interactable = true;
            Debug.Log("Player connected to network");
        }
    }
}
