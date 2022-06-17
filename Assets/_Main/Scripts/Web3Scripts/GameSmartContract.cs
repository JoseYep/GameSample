using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameSmartContract : MonoBehaviour
{
    [SerializeField] private string chain = "etherium";
    [SerializeField] private string network = "ropstein";
    [SerializeField] private string contract = "";
    [SerializeField] private string abiContract;

    public Action onComplete;
    public Action onFailed;

    private string gameId;

    protected string K3Y = "2538e703e05388f143a7629eb86dd6fb2c1af51ce6bd6d592ef6ff04495dcab2";

    //value in wei
    string value = "0";

    //gas limit
    string gasLimit = "";

    //gas price optional
    string gasPrice = "";

    string response = "";

    string[] abi_array = new string[1];

    public string GameId { get => gameId; set => gameId = value; }

    #region Contract Methods
    async public void GetGame()
    {
        gameId = String.Empty;

        try
        {
            response = await EVM.Call(chain, network, contract, abiContract, "gameId", "[]");
            gameId = response;

            Debug.Log("Game ID: " + gameId + "...");
        }
        catch (Exception e)
        {
            Debug.Log(e, this);
        }
    }

    async public void CreateGame(long money)
    {
        try
        {
            Debug.Log("Crating new game with bet: " + money);
            response = await Web3GL.SendContract("createGame", abiContract, contract, "[\"" + money + "\"]", (money * 10).ToString(), gasLimit, gasPrice);

            CheckStatus(response);
        }
        catch (Exception e)
        {
            onFailed?.Invoke();
            Debug.Log(e, this);
        }
    }

    async public void JoinGame(long money)
    {
        try
        {
            Debug.Log("Joining game: " + gameId + " with bet: " + money);
            response = await Web3GL.SendContract("joinGame", abiContract, contract, "[" + "\"" + money + "\" " + "," + "\"" + gameId + "\" " + "]", (money * 10).ToString(), gasLimit, gasPrice);
            CheckStatus(response);
        }
        catch (Exception e)
        {
            onFailed?.Invoke();
            Debug.Log(e, this);
        }
    }

    async public void Claim(string winner, string loser)
    {
        try
        {
            string sign = Web3PrivateKey.Sign(K3Y, "Claim");
            response = await Web3GL.SendContract("claim", abiContract, contract, "[" + "\"" + winner + "\" " + "," + "\"" + loser + "\" " + "," + "\"" + gameId + "\" " + "," + "\"" + sign + "\" " + "]","0", gasLimit, gasPrice);
            CheckStatus(response);
        }
        catch (Exception e)
        {
            onFailed?.Invoke();
            Debug.Log(e, this);
        }
    }

    async public void CheckStatus(string hash)
    {
        string confirmation;
        while (true)
        {
            confirmation = await EVM.TxStatus(chain, network, hash);

            Debug.Log("Transaction status: " + confirmation);

            if (confirmation == "success")
            {
                onComplete?.Invoke();
                Debug.Log(confirmation);
                break;
            }  
            else if(confirmation == "fail")
            {
                onFailed?.Invoke();
                Debug.Log(confirmation);
                break;
            }
        }
    }

    #endregion

    #region Converstion Methods
    private string ConvertToArgument(int value)
    {
        string convertion;
        convertion = String.Format("[\" {0} \"]", value);
        Debug.Log("Conversion Output: " + JsonConvert.SerializeObject(convertion));
        return JsonConvert.SerializeObject(convertion);
    }

    private string ConvertToArgument(int value, int value2)
    {
        string convertion;
        convertion = String.Format("[\" {0} \",\" {1} \" ]", value, value2);
        Debug.Log("Conversion Output: " + JsonConvert.SerializeObject(convertion));
        return JsonConvert.SerializeObject(convertion);
    }

    private string ConvertToArgument(string value, string value2, int value3, string value4)
    {
        string convertion;
        convertion = String.Format("[\" {0} \",\" {1} \",\" {2} \",\" {3} \" ]", value, value2, value3, value4);
        Debug.Log("Conversion Output: " + JsonConvert.SerializeObject(convertion));
        return JsonConvert.SerializeObject(convertion);
    }
    #endregion

    [ContextMenu("Test Conversion")]
    private void TestConversion()
    {
        Debug.Log("Format: " + "[\" {0} \"]");
        Debug.Log("Converted value: " + ConvertToArgument("13", "69", 420, "marimba"));
    }

    [ContextMenu("Test Conversion")]
    private void TestParams()
    {
        int i = 69;
        int j = 420;

        string s = "[" + "\"" + i + "\" " + "," +"\"" + j + "\" " +  "]";
        string o = "[\"" + "original" + "\"]";

        Debug.Log(o);
        Debug.Log(s);
    }
}
