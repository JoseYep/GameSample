using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SmartContract : MonoBehaviour
{
    [SerializeField] private string chain = "etherium";
    [SerializeField] private string network = "ropstein";
    [SerializeField] private string contract = "";
    [TextArea(2, 6)]
    [SerializeField] private int abi_number = 0;
    [SerializeField] private string abiContract;
    [SerializeField] string method = "";
    [SerializeField] string[] arguments;

    public Action onComplete;
    public Action onFailed;

    string args = "[\"12\",\"1\"]";

    //value in wei
    string value = "0";

    //gas limit
    string gasLimit = "";

    //gas price optional
    string gasPrice = "";

    string response = "";

    string[] abi_array = new string[1];

    #region Properties
    public string Chain { get => chain; set => chain = value; }
    public string Network { get => network; set => network = value; }
    public string Contract { get => contract; set => contract = value; }
    public int Abi { get => abi_number; set => abi_number = value; }
    public string Method { get => method; set => method = value; }
    public string Args { get => args; set => args = value; }
    public string Value { get => value; set => this.value = value; }
    public string GasLimit { get => gasLimit; set => gasLimit = value; }
    public string GasPrice { get => gasPrice; set => gasPrice = value; }
    public string Response { get => response; set => response = value; }
    #endregion

    async public void Run()
    {
        //abi_array[0] = "[{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"numero1\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"numero2\",\"type\":\"uint256\"}],\"name\":\"sumar\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"pure\",\"type\":\"function\"}]";
        abi_array[0] = abiContract;
        Debug.Log("Contract adress: " + contract);
        Debug.Log("Contract arg: " + args);
        Debug.Log("Contract abi: " + abi_array[abi_number]);
   
        string argsJ = JsonConvert.SerializeObject(arguments);
        
        Debug.Log("Contract args JSON: " + argsJ);

        try
        {
            //response = await Web3GL.SendContract(method, abi_array[abi_number], contract, args, "0",gasLimit, gasPrice);
            response = await Web3GL.SendContract(method, abiContract, contract, argsJ, "0",gasLimit, gasPrice);
            onComplete.Invoke();
        }
        catch (Exception e)
        {
            onFailed.Invoke();
            Debug.Log(e, this);
        }
    }
}
