using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNetworkAgent : MonoBehaviourPun
{
    [Header("Game Events")]
    [SerializeField] private GameEvent onPlayerPay;
    [SerializeField] private GameEvent onPlayerJoin;

    private PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();     
    }

    private IEnumerator Start()
    {    
        if (view.IsMine)
        {
            Debug.Log("Player network agent created");
            Joined();

            while (true)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    Debug.Log("Players connected");
                    break;
                }

                yield return null;
            }      
        }
    }

    private void Joined()
    {
        view.RPC("RPC_JoinedOnRoom", RpcTarget.All);
        Debug.Log("Player network agent joined");
    }

    public void PaymentComplete()
    {
        view.RPC("RPC_PaymentComplete", RpcTarget.All);
        Debug.Log("Player payment complete!");
    }
   
    #region RPCs
    [PunRPC]
    private void RPC_PaymentComplete()
    {
        onPlayerPay.Raise();
    }

    [PunRPC]
    private void RPC_JoinedOnRoom()
    {
        onPlayerJoin.Raise(gameObject);
    }

    #endregion
}
