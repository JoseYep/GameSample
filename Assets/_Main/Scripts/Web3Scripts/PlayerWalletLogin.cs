using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class PlayerWalletLogin : MonoBehaviour
{
    [SerializeField] private string chain;
    [SerializeField] private string network;

    [SerializeField] private GameObject accesBlocker;
    [SerializeField] private Button connectButton;
    [SerializeField] private TextMeshProUGUI walletId;
    [SerializeField] private TextMeshProUGUI walletBalance;

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
            GetData();
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

    async private void GetData()
    {
        account = PlayerPrefs.GetString("Account");
        walletId.text = "Wallet: " + account;
        string b = await EVM.BalanceOf(chain, network, account);
        Debug.Log("Wallet balance raw: " + b);

        float f = (float)((Int64.Parse(b)));
        
        Debug.Log("Wallet balance converted: " + Math.Round((f / 1000000000000000000),3));
        walletBalance.text = "Balance: " + Math.Round((f /1000000000000000000),3 ) + " MATIC";      
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

       GetData();

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
