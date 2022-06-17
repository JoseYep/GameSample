using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallABI : MonoBehaviour
{
    [SerializeField] private string chain = "etherium";
    [SerializeField] private string network = "ropstein";
    [SerializeField] private string contract = "0x87B902cf798d4b7f05Bc21FA48Dfc1606553A6A3";
    [TextArea(2,6)]
    [SerializeField] private string abi = "[{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"numero1\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"numero2\",\"type\":\"uint256\"}],\"name\":\"sumar\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"pure\",\"type\":\"function\"}]";


    private void Start()
    {
        string a = "12";
        string b = string.Format("[\"{0}\",\"1\"]", a);

        Debug.Log("[\"12\",\"1\"]");
        Debug.Log(b);
    }

    async public void AddValue()
    {
        //Smart contract method
        string method = "sumar";
        string args = "[\"12\",\"1\"]";

        //value in wei
        string value = "0";

        //gas limit
        string gasLimit = "";

        //gas price optional
        string gasPrice = "";

        string response = "";


        Debug.Log("Contract method" + method);
        Debug.Log("Contract args" + args);
        Debug.Log("Contract abi" + abi);

        //Connect to users wallet
        try
        {
            response = await Web3GL.SendContract(method, abi, contract, args, gasLimit, gasPrice);     
            
        } catch (Exception e) 
        {
            Debug.Log(e, this);
        }
    }
  
    async public void RetriveValue()
    {
        //Smart contract method
        string method = "retrive";
        string args = "[]";

        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args);
            Debug.Log(response);
        }
        catch (Exception e)
        {
            Debug.Log(e, this);
        }
    }
}
