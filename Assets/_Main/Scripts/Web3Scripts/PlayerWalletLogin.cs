using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerWalletLogin : MonoBehaviour
{
    [SerializeField] private GameObject accesBlocker;
    [SerializeField] private Button connectButton;

    [Header("Dev Test")]
    [SerializeField] private bool needLogin = true;

    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    private static bool loggedIn = false;
    private int expirationTime;
    private string account;
    private bool canPlay = false;

    private void Awake()
    {
        if (loggedIn)
        {
            accesBlocker.SetActive(false);
        }
        else
        {
            if (!needLogin)
            {
                accesBlocker.SetActive(false);

            }
            else
            {
                accesBlocker.SetActive(true);
            }
        }
    }

    public void OnLogin()
    {
        Web3Connect();
        OnConnected();
    }
    
    async private void OnConnected()
    {
        account = ConnectAccount();
        while (account == "")
        {
            await new WaitForSeconds(1f);
            account = ConnectAccount();
        };

        PlayerPrefs.SetString("Account", account);
 
        loggedIn = true;
        accesBlocker.SetActive(false);
        connectButton.gameObject.SetActive(false);
    }

    
    public void OnSkip()
    {
        loggedIn = false;
        PlayerPrefs.SetString("Account", "");
    }   
}
