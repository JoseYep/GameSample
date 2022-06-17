using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractRunner : MonoBehaviour
{
    [SerializeField] private SmartContractData contractData;


    public void Run(string methodName)
    {
        contractData.Run(methodName);
    }

    //public void Run(string contractMethod)
    //{
    //    foreach (MethodArgumentsModel model in contractData.Methods)
    //    {
    //        if (contractMethod == model.MethodName)
    //        {
    //            RunContract(model.MethodName, contractData.ContractABI, model.Arguments,contractData.GasLimit, contractData.GasPrice );
    //        }
    //    }
    //}
    
    async private void RunContract(string method, string abi, string args, string gasLimit, string gasPrice)
    {  
        try
        {
            Debug.Log("Interacting with contract");
            string response = await Web3GL.SendContract(method, abi, args, "0" ,gasLimit, gasPrice);
            Debug.Log(response);
        }
        catch (Exception e)
        {
            Debug.Log("ERROR " + e, this);
        }
    }
}
