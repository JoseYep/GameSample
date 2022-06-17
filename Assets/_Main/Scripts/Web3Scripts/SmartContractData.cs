using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MethodArgumentsModel 
{
    [SerializeField] private string methodName;
    [SerializeField] private string[] arguments;
    
    public string[] Arguments { get => arguments; set => arguments = value; }
    public string MethodName { get => methodName; set => methodName = value; }
    
}

[CreateAssetMenu(menuName = "Metaskins/SmartContract")]
public class SmartContractData: ScriptableObject
{
    [Header("SmartContract info")]
    [SerializeField] private string chain = "etherium";
    [SerializeField] private string network = "ropstein";
    [SerializeField] private string contractAdress = "";
    [TextArea(2,6)]
    [SerializeField] private string contractABI = "";
    
    //Smart contract method
    [SerializeField] private List<MethodArgumentsModel> methods;
    
    //value in wei
    private string value = "0";

    //gas limit
    private string gasLimit = "";

    //gas price optional
    private string gasPrice = "";

    string response = "";

    #region Parameters
    public string Chain { get => chain; set => chain = value; }
    public string Network { get => network; set => network = value; }
    public string ContractAdress { get => contractAdress; set => contractAdress = value; }
    public string ContractABI { get => contractABI; set => contractABI = value; }
    public List<MethodArgumentsModel> Methods { get => methods; set => methods = value; }
    public string Value { get => value; set => this.value = value; }
    public string GasLimit { get => gasLimit; set => gasLimit = value; }
    public string GasPrice { get => gasPrice; set => gasPrice = value; }
    public string Response { get => response; set => response = value; }
    #endregion
  
    public void Run(string methodName)
    {
        foreach (MethodArgumentsModel method in methods)
        {
            if (method.MethodName == methodName)
            {
                Run(method);
            }
        }
    }

    async private void Run(MethodArgumentsModel contractMethod)
    {
        string args = JsonConvert.SerializeObject(contractMethod.Arguments);

        try
        {
            Debug.Log("Interacting with contract");
            string response = await Web3GL.SendContract(contractMethod.MethodName, contractABI, args, "0", gasLimit, gasPrice);
            Debug.Log(response);
        }
        catch (Exception e)
        {
            Debug.Log("ERROR " + e, this);
        }
    }

}
