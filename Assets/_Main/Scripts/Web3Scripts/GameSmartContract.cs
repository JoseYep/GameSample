using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameSmartContract : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string Claim(string winner, string loser, string GameID, string key);
    
    [SerializeField] private string chain = "etherium";
    [SerializeField] private string network = "ropstein";
    [SerializeField] private string contract = "";
    [SerializeField] private string abiContract;

    public Action onComplete;
    public Action onFailed;

    private string gameId;

    protected string K3Y = "0xa7bc7a3653717c2418195e3681ea9616b2d5b0f90164d7116c0bfadafb80f590";

    string rpc = "https://matic-mumbai.chainstacklabs.com/";

    //value in wei
    string value = "";

    //gas limit
    string gasLimit = "";

    //gas price optional
    string gasPrice = "";

    string[] abi_array = new string[1];

    public string GameId { get => gameId;}

    #region Contract Methods
    async public void GetGame()
    {
        gameId = String.Empty;

        try
        {
            string response = await EVM.Call(chain, network, contract, abiContract, "gameId","[]" ,rpc);
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
            gasPrice = await EVM.GasPrice(chain, network, rpc);
            //gasLimit = "75000";

            Debug.Log("Crating new game with bet: " + money);
            string[] parameters = {money.ToString()};
            string args = JsonConvert.SerializeObject(parameters);
            string val = (money + 100).ToString();

            string response = await Web3GL.SendContract("createGame", abiContract, contract, args, val , gasLimit, gasPrice);

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
            #region Get Game ID
            gameId = String.Empty;

            try
            {
                string gameId = await EVM.Call(chain, network, contract, abiContract, "gameId", "[]");
            }
            catch (Exception e)
            {
                Debug.Log(e, this);
            }

            #endregion

            gasPrice = await EVM.GasPrice(chain, network, rpc);
            //gasLimit = "75000";

            Debug.Log("Joining game: " + gameId + " with bet: " + money);
            string[] parameters = {money.ToString(), gameId };
            string args = JsonConvert.SerializeObject(parameters);
            string val = (money + 100).ToString();
            
            string response = await Web3GL.SendContract("joinGame", abiContract, contract, args , val ,gasLimit, gasPrice);
            
            CheckStatus(response);
        }
        catch (Exception e)
        {
            onFailed?.Invoke();
            Debug.Log(e, this);
        }
    }

    async public void ClaimReward(string winner, string loser)
    {
        try
        {
            #region Get Game ID
            gameId = String.Empty;

            try
            {
                string gameId = await EVM.Call(chain, network, contract, abiContract, "gameId", "[]", rpc);
                Debug.Log("Game ID: " + gameId + "...");
            }
            catch (Exception e)
            {
                Debug.Log(e, this);
            }

            Debug.Log("Claiming reward for game " + gameId);
            #endregion

            #region Send721
            string account = Web3PrivateKey.Address(K3Y);

            string[] obj = { winner, loser, gameId};
            string args = JsonConvert.SerializeObject(obj);

            Debug.Log("Args: " + args);
            string chainId = await EVM.ChainId(chain, network, rpc);
            string gas = await EVM.GasPrice(chain, network, rpc);

            string data = await EVM.CreateContractData(abiContract, "claim", args);
            Debug.Log("Contract data: " + data);

            gasLimit = "75000";
            string transaction = await EVM.CreateTransaction(chain, network, account, contract, value, data, gasPrice, gasLimit, rpc);
            Debug.Log("transaction: " + transaction);

            string signature = Web3PrivateKey.SignTransaction(K3Y, transaction, chainId);
            Debug.Log("Signature: " + signature);

            string response = await EVM.BroadcastTransaction(chain, network, account, contract, value, data, signature, gasPrice, gasLimit, rpc);

            new WaitForSeconds(1f);
            CheckStatus(response);
            #endregion
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
        Debug.Log("Transaction Hash: " + hash);
        
        while (true)
        {
            if (IsValidHash(hash))
            {

                new WaitForSeconds(1f);

                confirmation = await EVM.TxStatus(chain, network, hash);
                Debug.Log("Transaction status: " + confirmation);

                if (confirmation == "success")
                {
                    onComplete?.Invoke();
                    Debug.Log(confirmation);
                    break;
                }
                else if (confirmation == "fail")
                {
                    onFailed?.Invoke();
                    Debug.Log(confirmation);
                    break;
                }
                else if (confirmation == "Returned error")
                {
                    onFailed?.Invoke();
                    Debug.Log(confirmation);
                    break;
                }
                else if (confirmation == "Returned")
                {
                    onFailed?.Invoke();
                    Debug.Log(confirmation);
                    break;
                }
                else if (confirmation == "returned")
                {
                    onFailed?.Invoke();
                    Debug.Log(confirmation);
                    break;
                }
                else if (confirmation == "error")
                {
                    onFailed?.Invoke();
                    Debug.Log(confirmation);
                    break;
                }
            }
            else
            {
                Debug.LogError("ERROR: Hash format incorrect! " + hash);
                break;
            }            
        }
    }

    private bool IsValidHash(string hash)
    {
        string s = "0x";
        return hash.Contains(s);
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
